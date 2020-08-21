# HFM.NET - Client Monitoring Application for the Folding@Home Distributed Computing Project

[![Build status](https://harlam357.visualstudio.com/hfm-net/_apis/build/status/hfm-net%20(master))](https://harlam357.visualstudio.com/hfm-net/_build/latest?definitionId=0)

Download here on GitHub - https://github.com/harlam357/hfm-net/releases

Download from Google Drive - https://drive.google.com/open?id=0B8d5F59S5sCiS1RISzdsaEd5UXM&authuser=0

## Version 0.9.17.1040

### Release Date: May 1, 2020

I encourage everyone to save their hfmx configuration file once loaded into 0.9.17.  Just select File > Save Configuration.  I did not make this automatic because it does create an hfmx file that is incompatible with the previous release version 0.9.12.  However, taking this step will generate unique identifiers for each client which enhances the link between the client definition and slot benchmarks.

* Enhancement: Slot Type now reports actual threads for CPU slots.
* Enhancement: Benchmarks captures and tracks individual GPU or CPU + threads.
* Enhancement: Slots grid painting rewritten (b/c the old code was a travesty).  The main change you'll see is PPD and Credit is now right justified and the number formatting is applied.  So much easier to read.
* Enhancement: Client hostname/IP and port echoed in status bar.

* Fix: Ensure Main and Benchmarks windows are restored in the viewable screen.
* Fix: Fix to allow log viewers like notepad++ to be used.

* Change: Changed many of the labels in the UI to align better with current FAH terminology.
* Change: Remove "Allow Asynchronous Clocks" setting (v6 thing).
* Change: Use the new HFM.Client API NuGet package.
* Change: Requires .NET Framework v4.7.
