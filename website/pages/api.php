<div id="content" style="text-align: center">

<h2>API</h2>

<br />
<p>
The status page uses an API to contact the backend for information about the status
and the servers being listed. This backend is open for other developers to be used
in their own applications.
</p>

<br />

<p>
The backend API outputs all the data as a JSON string.
</p>

<br />

<p style="font-size: 1.3em">
API URLs
</p>

<br />

<p>
<ul>
<li><code>http://s.mufff.in/api/cache</code> - Outputs all the available cached data from the backend.</li>
<li><code>http://s.mufff.in/api/status</code> - Outputs only the status indications (it's all boolean)</li>
<li><code>http://s.mufff.in/api/serverlist</code> [not working yet] - Server lists only, use <code>/api/cache</code> for now instead.</li>
</ul>
</p>

</div>
