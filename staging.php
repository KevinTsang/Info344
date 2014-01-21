<?php
	if (!isset($_REQUEST['name']))
	{
		echo "No results found.";
		die();
	}
	$playernamequery = '%' . $_REQUEST['name'] . '%';
	try {
		$dbObject = new PDO('mysql:host=info344dbinstance.c4xrwkjgcduy.us-west-2.rds.amazonaws.com;
							port=3306;
							dbname=info344dbinstance;
							', 'info344user', '3Brothers');
		$query = $dbObject->prepare("SELECT * FROM playerdata WHERE PlayerName LIKE :placeholder");
		$query->execute(array('placeholder' => $playernamequery));
		if ($query->rowCount() == 0)
		{
			echo 'No results found.';
			die();
		}

		while ($row = $query->fetch(PDO::FETCH_ASSOC))
		{
?>
			<div class='resultsCell'>
				<?php
					$playername = $row['PlayerName'];
					//$twitterurl = 'https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name=' . urlencode($playername);
					//$twitterfeed = json_decode($twitterurl);
					$array = explode(' ', $playername);
					$playername = implode('_', $array);
					$playername = strtolower($playername);
					$playername = str_replace('.', '', $playername);
					$playerpicture = 'http://i.cdn.turner.com/nba/nba/.element/img/2.0/sect/statscube/players/large/' . $playername . '.png';
				?>
				<img class='playerpicture' src=<?php echo $playerpicture; ?>></img>
				<div class='playerdata'><?php echo $row['PlayerName'];?>
					<table>
						<thead>
							<tr>
								<th>GP</th>
								<th>FGP</th>
								<th>TPP</th>
								<th>FTP</th>
								<th>PPG</th>
							</tr>
						</thead>
						<tbody>
							<tr>
								<td><?php if ($row['GP'] == 0) { echo 'N/A'; } else { echo $row['GP']; } ?></td>
								<td><?php if ($row['FGP'] == 0) { echo 'N/A'; } else { echo $row['FGP']; } ?></td>
								<td><?php if ($row['TPP'] == 0) { echo 'N/A'; } else { echo $row['TPP']; } ?></td>
								<td><?php if ($row['FTP'] == 0) { echo 'N/A'; } else { echo $row['FTP']; } ?></td>
								<td><?php if ($row['PPG'] == 0) { echo 'N/A'; } else { echo $row['PPG']; } ?></td>
							</tr>
						</tbody>
					</table>
				</div>
				<div>
					<?php
						//print_r($twitterfeed);
					?>
				</div>
			</div>
<?php 	}
	}
	catch (PDOException $e)
	{
		echo 'Error: ' . $e->getMessage();
	}
?>