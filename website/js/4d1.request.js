function init_cache_request()
{
	requestCache();
	setInterval(requestCache, 30000);
}

$(document).ready(init_cache_request);