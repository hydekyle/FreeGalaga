<?php
    header("Access-Control-Allow-Origin: *");

    define('DB_NAME', 'palolto_spgame');
    define('DB_USER', 'mktlogic_game_ad');
    define('DB_PASSWORD', 'eM93bn#6');
    define('DB_HOST', 'localhost');

    $table_name = "registro";
    $column_alias = "reg_alias";
    $column_intentos = "reg_intentos";
    
    $alias = $_GET["alias"];
    $intentos = $_GET["intentos"];

    $update_intentos_query = "UPDATE $table_name SET $column_intentos = $intentos WHERE $column_alias = '$alias'";

    $mysqli = new mysqli(DB_HOST, DB_USER, DB_PASSWORD, DB_NAME);

    if ($mysqli->connect_errno) exit ("Error de conexión");

    if (!$resultado = $mysqli->query($update_intentos_query)) die ("Error de consulta");

    die("OK");
    
?>
