using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZObjectPools;

public class Player : MonoBehaviour
{
    public GameObject shotPrefab;
    public Stats stats;
    EZObjectPool playerShots;
    float lastTimeShot;
    float minPosX, maxPosX, minPosY = -5f, maxPosY = -2f;
    AudioClip shotAudioClip;
    Transform gunPoint;
    public BoxCollider2D myCollider;
    public int playerLevel = 1;
    SpriteRenderer spriteRenderer;
    public Shield shield;
    public float lastTimeAttackBoosted = -10f;
    public bool vulnerable = true;
    float attackBoostTime = 7f;

    private void Start ()
    {
        Initialize ();
    }

    void Initialize ()
    {
        bulletMaterial.color = Color.white;
        minPosX = GameManager.Instance.minPosX;
        maxPosX = GameManager.Instance.maxPosX;
        spriteRenderer = GetComponent<SpriteRenderer> ();
        myCollider = GetComponent<BoxCollider2D> ();
        gunPoint = transform.Find ("GunPoint");
        shotAudioClip = AudioManager.Instance.scriptableSounds.basicShot;
        SetBulletsPool (0);
    }

    void SetBulletsPool (int level)
    {
        if (level > GameManager.Instance.tablesEtc.disparosJugador.Count) return;
        playerShots?.ClearPool ();
        var newShotPrefab = GameManager.Instance.tablesEtc.disparosJugador [level];
        playerShots = EZObjectPool.CreateObjectPool (newShotPrefab, "PlayerShots" + level, 15, true, true, true);
    }

    void Update ()
    {
        Vector3 deltaPosition = new Vector3 (Mathf.Clamp (Input.GetAxis ("Horizontal"), -1f, 1f), Mathf.Clamp (Input.GetAxis ("Vertical"), -1f, 1f), 0);
        transform.position += deltaPosition * Time.deltaTime * stats.movementVelocity;
        transform.position = new Vector3 (Mathf.Clamp (transform.position.x, minPosX, maxPosX), Mathf.Clamp (transform.position.y, minPosY, maxPosY), 0);

        if (isAttackBoosted && Time.time > lastTimeAttackBoosted + attackBoostTime) DesactivateAttackBoost ();
        else
        {
            float fillValue = Mathf.Clamp ((lastTimeAttackBoosted + attackBoostTime - Time.time) * 2 / 10f, 0f, 1f);
            CanvasManager.Instance.SetFillBoostIcon (fillValue);
        }
    }

    public void Shoot ()
    {
        if (isShootAvailable () && playerShots.TryGetNextObject (GetGunPosition (), Quaternion.identity, out GameObject go))
        {
            go.GetComponent<Rigidbody2D> ().velocity = Vector2.up * stats.shootSpeed * 10;
            lastTimeShot = Time.time;
            AudioManager.Instance.PlayPlayerShot ();
        }
    }

    bool lastShootRight;
    Vector3 GetGunPosition ()
    {
        if (playerLevel == 4)
        {
            lastShootRight = !lastShootRight;
            return gunPoint.transform.position + (lastShootRight ? Vector3.right / 3 : Vector3.left / 3);
        }
        return gunPoint.transform.position;
    }

    bool isShootAvailable ()
    {
        return Time.time > lastTimeShot + 0.4f / stats.shootCooldown / (isAttackBoosted ? 2 : 1);
    }

    void GetStrike ()
    {
        vulnerable = false;
        GameManager.Instance.LoseLives (1, 1.5f);
    }

    public IEnumerator InmuneTime (float inmuneTime)
    {
        var lastLevel = GameManager.Instance.activeLevelNumber;
        var lastSpeed = EnemiesManager.Instance.animationSpeed;

        StartCoroutine (BlinkTime (inmuneTime));
        yield return new WaitForSeconds (inmuneTime);

        vulnerable = true; // El collider se desactiva antes de entrar en la Corutina para evitar múltiples hits.
    }

    public IEnumerator BlinkTime (float blinkTime)
    {
        //var startTime = Time.time;
        var endTime = Time.time + blinkTime;
        var defaultColor = Color.white;
        var invisibleColor = defaultColor;
        invisibleColor.a = 0f;

        while (Time.time < endTime)
        {
            spriteRenderer.color = spriteRenderer.color == invisibleColor ? defaultColor : invisibleColor;
            yield return new WaitForSeconds (0.12f);
        }
        spriteRenderer.color = Color.white;
    }

    public void LevelUp ()
    {
        if (GameManager.Instance.lives == 0) return;
        AudioManager.Instance.PlayPlayerLevelUp ();
        playerLevel++;
        stats.movementVelocity += 0.75f;
        stats.shootSpeed += 1;
        stats.shootCooldown += 1;
        stats.damage += 1;

        SetBulletsPool (playerLevel - 1);

        try
        {
            GetComponent<SpriteRenderer> ().sprite = GameManager.Instance.tablesEtc.navesJugador [playerLevel - 1];

        }
        catch
        { }
    }

    void GetShield ()
    {
        AudioManager.Instance.PlayAudioPlayer (GameManager.Instance.tablesSounds.lifeUp);
        shield.ActivateShield ();
    }

    void GetPoints ()
    {
        AudioManager.Instance.PlayAudioPlayer (GameManager.Instance.tablesSounds.lifeUp);
        CanvasManager.Instance.AddScore (1000);
    }

    void GetHealth ()
    {
        AudioManager.Instance.PlayAudioPlayer (GameManager.Instance.tablesSounds.lifeUp);
        GameManager.Instance.GainLives (1);
    }

    public Material bulletMaterial;

    void GetAttackBoost ()
    {
        AudioManager.Instance.PlayAudioPlayer (GameManager.Instance.tablesSounds.lifeUp);
        //stats.shootCooldown++;
        lastTimeAttackBoosted = Time.time;
        isAttackBoosted = true;
        bulletMaterial.color = Color.red;
    }

    void DesactivateAttackBoost ()
    {
        isAttackBoosted = false;
        bulletMaterial.color = Color.white;
    }

    public bool isAttackBoosted = false;

    // public bool IsAttackBoosted ()
    // {
    //     return Time.time < lastTimeAttackBoosted + 6.66f;
    // }

    private void OnTriggerStay2D (Collider2D other)
    {
        if (other.CompareTag ("Enemy"))
        {
            if (vulnerable)
            {
                other.GetComponent<Enemy> ().Erase ();
                GetStrike ();
            }
        }
        else if (other.CompareTag ("EnemyShot"))
        {
            if (!vulnerable) return;
            other.gameObject.SetActive (false);
            GetStrike ();
            EnemiesManager.Instance.ReloadShootCooldown ();
        }
        else if (other.CompareTag ("PowerUp"))
        {
            var boostType = other.GetComponent<PowerUp> ().boostType;
            switch (boostType)
            {
                case BoostType.Shield:
                    GetShield ();
                    break;
                case BoostType.Points:
                    GetPoints ();
                    break;
                case BoostType.AttackSpeed:
                    GetAttackBoost ();
                    break;
                case BoostType.Health:
                    GetHealth ();
                    break;
            }
            Destroy (other.gameObject);
        }
    }

}
