var status_led_path = "http://cdn1.iconfinder.com/data/icons/softwaredemo/PNG"; //<width>x<height>/Box_<color>.png";

function capitaliseFirstLetter(string)
{
	return string.charAt(0).toUpperCase() + string.slice(1);
}

function bool2int(value)
{
	return value ? 1 : 0;
}

function led(color)
{
	color = capitaliseFirstLetter(color);
	return status_led_path + "/64x64/Box_" + color + ".png";
}

function niceAnimation(id)
{
	$(id +">img[src='" + red + "']").attr("src", "");
	$(id +">img[src='" + grey + "']").attr("src", red);
	$(id +">img[src='']").attr("src", grey);
}

function statusHeader(suffix, title, description)
{
	if(suffix.length > 1)
		var color = suffix.substring(1);
	else
		var color = "grey";
	$("#header_repeat_bg").css("background-image", "url(css/images/header_bg_repeat" + suffix + ".png)");
	$("#header_bg").css("background-image", "url(css/images/header_bg" + suffix + ".jpg)");
	$("#header_title").html(title);
	$("#header_text").html(description);
	$("#header_repeat_bg").fadeIn("fast");
	$("#fdocol").animate( { "color":color }, "slow");
	$("#menu > p").fadeIn("slow");
}

function requestCache()
{
	$.ajax({
		url:"/api/cache",
		cache:false,
		dataType:"json",
		success:parseCache,
		error: function(jqXHR, textStatus, errorThrown) {
			console.log("/api/cache - Error thrown: " + errorThrown);
			console.log("/api/cache - Text status: " + textStatus);
			if(window.location.pathname == "/status") $("#content").fadeOut("slow");
			statusHeader(
				"",
				"Status page down",
				"The backend serving the status is not available at the moment due to maintenance or a crash server-side. The page will be back with all info somewhen later on."
			);
		}
	});
}

function parseCache(d)
{
	if(!checkStatusAvailable(d))
	{
		if(window.location.pathname == "/status") $("#content").fadeOut("slow");

		statusHeader(
			"",
			"Status page is slowly coming back...",
			"The backend just restarted and needs another 1-2 minutes to come back. Be patient!"
		);
		return;
	}

	if(window.location.pathname == "/status")
		$("#content").fadeIn("fast");

	$("#fdoss").text(d["backend-name"]);

	server("iw4m-np", d["iw4m"]["NPOnline"], d["iw4m"]["NPCounter"], d["iw4m"]["CacheIntervals"]["np"]);
	server("iw4m-master", d["iw4m"]["MasterOnline"], d["iw4m"]["MasterCounter"], d["iw4m"]["CacheIntervals"]["master"]);

	server("iw5m-np", d["iw5m"]["NPOnline"], d["iw5m"]["NPCounter"], d["iw5m"]["CacheIntervals"]["np"]);
	server("iw5m-master", d["iw5m"]["MasterOnline"], d["iw5m"]["MasterCounter"], d["iw5m"]["CacheIntervals"]["master"]);

	server("4d1-forum", d["forum"]["Online"], -1, -1);
	server("4d1-auth-internal", d["login"]["AuthInternalOnline"], d["login"]["AuthInternalCounter"], d["login"]["CacheIntervals"]["auth-internal"]);
	server("4d1-auth", d["login"]["AuthOnline"], d["login"]["AuthCounter"], d["login"]["CacheIntervals"]["auth"]);

	server("kms-host", d["kmshost"]["KmshostOnline"], -1, -1);

	var level = 0
/*
		+ (5 * d["iw4m"]["NPCounter"] / 7)
		+ (2 * d["iw4m"]["MasterCounter"] / 7)
		+ (5 * d["iw5m"]["NPCounter"] / 7)
		+ (2 * d["iw5m"]["MasterCounter"] / 7)
		+ (1 * bool2int(d["forum"]["Online"]))
		+ (3 * d["login"]["AuthCounter"] / 6)
		+ (2 * d["login"]["AuthInternalCounter"] / 6);
*/
		+ (5 * bool2int(d["iw4m"]["NPOnline"]))
		+ (2 * bool2int(d["iw4m"]["MasterOnline"]))
		+ (5 * bool2int(d["iw5m"]["NPOnline"]))
		+ (2 * bool2int(d["iw5m"]["MasterOnline"]))
		+ (1 * bool2int(d["forum"]["Online"]))
		+ (3 * bool2int(d["login"]["AuthOnline"]))
		+ (2 * bool2int(d["login"]["AuthInternalOnline"]));

	var suffix = ".green";
	var title = "It's working";
	var description = "No issues have been detected yet.";
	if(level <= 10)
	{
		title = "Heavy issues";
		description = "The status page detected some severe problems on the fourDeltaOne interfaces, check below to see which interfaces are affected.<br />"
			+ "If you have nothing to do, try <a href=\"random\">this</a>.";
		suffix = ".red";
	}
	else if(level < 20)
	{
		title = "Problems detected";
		description = "fourDeltaOne has problems on some interfaces, check below to see which interfaces are affected by the problem.";
		suffix = ".yellow";
	}

	if(window.location.search.indexOf("level") != -1)
		description = "Operation level " + level + " / 20<br />" + description;

	//if(window.location.search.indexOf("debug") == -1)
	//	statusHeader("", "Okay.", "Would you stop writing s.mufff.in in the chat? Thank you.");
	//else
		statusHeader(suffix, title, description);
}

function checkStatusAvailable(d)
{
	d = d["iw4m"];
	var dateic = new Date();
	if(d == null || d == "")
	{
		return false;
	} else {
		dateic.setISO8601(d["InstanceCreateTime"]);
		var now = new Date();
		var difference = Math.ceil((now.getTime() - dateic.getTime()) / 1000);
		if(difference < 90)
			return false;
	}
	return true;
}

function server(id, isOnline, counter, interval, offlinetext)
{
	if(window.location.search.indexOf("debug") != -1)
		console.log(id + " is " + (isOnline ? "" : "not ") + "online with a counter of " + counter + " and interval of " + interval + "s; counter * interval = " + (counter * interval) + "s");
	var e = $("#led-" + id);
	e.attr("src", isOnline ? led("green") : (interval > 0) ? (counter * interval > 90 ? led("yellow") : led("red")) : led("red"));
	e.attr("level", counter * interval);
	/*if(!isOnline)
	{
		$("#" + category + "-check>li").last().after($("<li class='server-off'>" + offlinetext + "</li>"));
		//if(counter > 2)
		//  $("#" + category + "-check>li").last().after($("<li>" + servername + " detected as <em>possibly</em> online.</li>"));
	}*/
}

