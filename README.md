# OCCprocess

## Introduction
OCCprocess is used to process accelerated options. The name of the application is OCCprocess.exe and takes no parameters.

## Quick Start Guide
Process Execution:  > OCCprocess.exe 

•	Application parses through each record in the security update XML file. If the record represents an accelerated option, the option is updated.

•	The number of records that were successfully processed is shown after the file is parsed.

## Files

•	Use OCCprocess.exe located in //IC/IC_DATA/exes/OCCprocess/

The folder will contain the application along with its dependencies.

• Input File: //IC/IC_DATA/load/SEC_UPDATE.xml

• Log File: //IC/IC_DATA/log folder

Log file name is set in Config file.

## Application Command
Command to execute the process: "//IC/IC_DATA/exes/OCCprocess/OCCprocess.exe"

## Servers Deployed
FUTU UAT - 49.51.84.242

## Application Document
https://inteliclear.sharepoint.com/:w:/r/Shared%20Documents/IC%20System%20Documents/IC_OCCprocess.docx?d=w4ccc2b9baf3c4b5292d327b70918bac9&csf=1&web=1
