<?php 
	if (!isset($_GET["n"]))
		die();
	$number = $_GET["n"];
	for ($i = 2; $i <= $number; $i = $i + 2)
	{
		echo $i . " ";
	}

	// $arr = array();
	// $index = 0;
	// for ($i = floor(sqrt($number)); $i > 0; $i++)
	// {
	// 	if ($number % $i == 0)
	// 	{
	// 		$number = $number / $i;
	// 	}
	// }
?>
