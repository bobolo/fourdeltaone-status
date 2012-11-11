FourDeltaOne Status Project
===========================

These are the source codes for the fourDeltaOne status project I work on. I
recently decided to make the source codes public since most of the project
became stable enough and I like to share (cleaned up and working) code.

What does this repository include
=================================

Right at the moment, this repository includes the source code from the following
parts of the project:

- Status backend (API)  
- Status frontend (Webpage)

Following parts are still to be included completely:

- Status bot (IRC bot)

I want to have this on my own server!
=====================================

Sorry, but without my help you won't get it running if you don't know what you
need to do.

1. It's the least important problem, but you will need to compile the latest
   mono (or at least 2.11) to get the backend running since it uses .NET 4.
2. You can NOT run the website on an nginx server.
3. You MUST configure special rules for a VirtualHost on an apache server if you
   want to get the website running, for example ProxyPass/ProxyPassReverse for
   allowing connections to the backend.
4. You also will need Deathmax's server list backend, which I did not include
   for obvious reasons.
   
With other words: If you want this page on your server, let me know on Rizon
IRC in the #fourdeltaone channel or via PM/memo. I will then talk with you about
it (normally I approve instantly :P) and then I can set the page and the back-
end for you.

License
=======

The project is published under the terms of the GNU General Public License
Version 3, you can read the whole text in the LICENSE.txt.