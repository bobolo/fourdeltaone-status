<?php
if(empty($_GET["page"]) || !file_exists("pages/".$_GET["page"].".php"))
{
	header("location: status");
	exit;
}

define("ROOT", dirname(__FILE__));

require_once("includes/html_api.php");

$menu = Array(
	"links"=>Array(
		page("status", "Status", "status"),
		page("iw4msl", "IW4M servers", "iw4msl"),
		page("iw5msl", "IW5M servers", "iw5msl"),
		page("random", "I'm bored", "random"),
		page("4d1", "FourDeltaone", "http://fourdeltaone.net/")
	)
);

function gen_leds($titles)
{
?><table style="width: 100%">
<? foreach($titles as $id=>$title) { ?>
<tr>
<td class="status-led led-td-<?=$id ?>"><img id="led-<?=$id ?>" width="64" height="64" title="<?=$title ?>" alt="<?=$title ?>" src="http://cdn1.iconfinder.com/data/icons/softwaredemo/PNG/64x64/Box_Grey.png" /></td>
<td class="status-text text-<?=$id ?>" style="font-size: 16px; text-align: left"><?=htmlentities($title) ?></td>
</tr>
<? } ?>
</table><?
}

?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<? include("templates/meta.html"); ?>
<title>fourdeltaone.<?=$_GET["page"] ?></title>
<? include("templates/fonts.html"); ?>
<? include("templates/styles.html"); ?>
<? include("templates/scripts.html"); ?>
</head>

<body>
	<div id="menu_bg">
		<div id="menu">
		<? menu(); ?>
		<p style="padding: 20px 5px 5px 5px; font-size: 32px; font-family: Electrolize; display: none">
			<span id="fdocol">fourdeltaone</span>.<?=$_GET["page"] ?>
		</p>
		</div>
	</div>
<!--
	<div id="logo" style="display: none">
	</div>
-->

	<? require "pages/".$_GET["page"].".php"; ?>

<div style="text-align: center; font-size: 8px; padding-top: 50px; color: #444">
	<p>This status page is being maintained by Icedream, not by the 4D1 staff. &copy; 2012 Icedream</p>
</div>
					<? /*
	<div id="footer_repeat_bg" style="font-size: 8px">
		<div id="footer_bg">
			<div id="footer_width">
				<div id="footer_top">
					<div id="footer_left">
						<h2>About Us</h2>
							<img src="images/img1.jpg" alt="" title="" style="float: left; padding: 0px 20px 10px 0px"/>
							<p><a href="#">Morbi id vehicula orci.</a> 
Ut consequat commodo nunc nec tincidunt. Pellentesque vitae gravida nulla. Aliquam fermentum ipsum et mauris rutrum ac ornare convallis justo in eros fermentum eget cursus augue cursus. Donec sit amet eros eget ligula blandit congue. </p>
						</div>
						<div id="footer_middle">
							<h2>Recent Posts</h2>
							<ul class="ul_hover_bg">
								<li><a href="#">Lorem ipsum dolor sit amet, consectetur adip</a></li>
								<li><a href="#">Quisque nec lectus leo, et condimentum massa.</a></li>
								<li><a href="#">Suspendisse porttitor purus a nisl tincidunt at </a></li>
								<li><a href="#">Aliquam et leo quis massa ultricies varius non </a></li>
								<li><a href="#">Morbi eget arcu metus, facilisis rhoncus mi.</a></li>
								<li><a href="#">Morbi condimentum enim in lorem iaculis ultr</a></li>
							</ul>
						</div>
						<div id="footer_right">
							<h2>Contact Form</h2>
							<form id="form1" method="post" action="#">
                            	<fieldset>
                                	<label>Name:</label><input id="text1" type="text" name="text1" value="" alt=""/>
									<div class="clear"></div>
                                	<label>E-mail:</label><input id="text2" type="text" name="text2" value="" alt=""/><br />
                                	<textarea id="text_mess" name="text_mess" cols="0" rows="0"></textarea><br />
                            	    <input type="submit" id="login-submit" value="Send"/>
                               	</fieldset>
                            </form>
						</div>
						<div class="clear"></div>
					</div>
					<div id="footer_bot">
						<div id="footer_bot_left">
							<p>Copyright  2011. <!-- Do not remove -->Designed by <a href="http://www.metamorphozis.com/free_templates/free_templates.php" title="Free Web Templates">Free Web Templates</a>, coded by <a href="http://www.myfreecsstemplates.com/" title="Free CSS Templates">Free CSS Templates</a><!-- end --></p>
		                    <p><a href="#">Privacy Policy</a> | <a href="#">Terms of Use</a> | <a href="http://validator.w3.org/check/referer" title="This page validates as XHTML 1.0 Transitional"><abbr title="eXtensible HyperText Markup Language">XHTML</abbr></a> | <a href="http://jigsaw.w3.org/css-validator/check/referer" title="This page validates as CSS"><abbr title="Cascading Style Sheets">CSS</abbr></a></p>		
						</div>
						<div id="footer_icon">
							<a href="#"><img src="images/facebook.png" alt="" title=""/></a>
							<a href="#"><img src="images/twitter.png" alt="" title=""/></a>
							<a href="#"><img src="images/yahoo.png" alt="" title=""/></a>
							<a href="#"><img src="images/rss.png" alt="" title=""/></a>
							<a href="#"><img src="images/youtube.png" alt="" title=""/></a>
						</div>
					</div>
					<div class="clear"></div>
				</div>
			</div>
		</div>
*/ ?>
    </body>
</html>
