<?php
    header("Access-Control-Allow-Origin: *");
    define('DB_NAME', 'palolto_spgame');
    define('DB_USER', 'mktlogic_game_ad');
    define('DB_PASSWORD', 'eM93bn#6');
    define('DB_HOST', 'localhost');

    $alias = $_GET["alias"];
    $score = $_GET["score"];

    $table_name = "registro";
    $column_alias = "reg_alias";
    $column_score = "reg_score";

    $update_score_query = "UPDATE $table_name SET $column_score = $score WHERE $column_alias = '$alias'";
    $get_user_query = "SELECT * FROM $table_name WHERE $column_alias = '$alias'";

    $mysqli = new mysqli(DB_HOST, DB_USER, DB_PASSWORD, DB_NAME);

    if ($mysqli->connect_errno) exit("Error de conexión");

    if (!$resultado = $mysqli->query($get_user_query)) die("Error de consulta");

    if ($resultado->num_rows === 0) {
        die("No se encontró al usuario");
    } else {
        $storedUser = $resultado->fetch_array();
	if ($storedUser[$column_score] < $score) {
		if ($updated_user = $mysqli->query($update_score_query)) die ("Usuario Updated");
	}else die ("Record no superado");

    }

?>
