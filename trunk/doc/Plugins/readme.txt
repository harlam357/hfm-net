INSTALLING
----------
To install an HFM.NET Plugin you must place the Plugin dll file in the 
following folder on your PC.  This folder WILL NOT be created for you  
by HFM.  You must create the folder yourself.

- Windows XP: C:\Documents and Settings\<username>\Application Data\HFM\Plugins

- Windows Vista or 7: C:\Users\<username>\AppData\Roaming\HFM\Plugins

- Linux (Mono): /home/<username>/.config/HFM/Plugins

The Plugin dll must be in place when HFM is started.  Otherwise, you 
need to exit and restart HFM in order for the Plugin to be detected.


DEVELOPING
----------
The only Plugins currently supported are for reading and writing a client
configuration file.  There is no official documentation on this interface
at this time.  However, the default HFM.NET client config serializer class 
is written as a Plugin and you can use it as a template to develop your own
Plugin.  The implementation (ClientInstanceXmlSerializer.cs) can be found 
in the HFM.NET SVN source repository.

http://code.google.com/p/hfm-net/source/checkout
