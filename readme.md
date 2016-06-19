# HFM.NET - Client Monitoring Application for the Folding@Home Distributed Computing Project

Download from Google Drive - https://drive.google.com/open?id=0B8d5F59S5sCiS1RISzdsaEd5UXM&authuser=0

## Version 0.9.6.501

### Release Date: June 19, 2016

* Fix: Change the default Project Summary download URL for new installs to http://assign.stanford.edu/api/project/summary.

* Enhancement: Right-click context menu now includes commands to Fold, Pause, and Finish the FAH client slot.
* Enhancement: Work Unit History data can now be exported to csv through the File menu of the Work Unit History window.

* Change: Reworked HFM's FAH v7 client interface API.
* Change: Reworked FAH v7 client handling code.  Testing has shown faster retrieval times of the log.txt file through the FAH v7 client API.


## Version 0.9.5.478

### Release Date: February 7, 2016

* Fix/Update: Use project summary JSON feed for project (work unit) information in lieu of parsing psummary.html.