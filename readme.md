# WebDesktop

WebDesktop puts a transparent over every monitor screen and then draws a webpage onto them, you can use this functionality to build desktop widgets entirely in a web stack.

### Warning

This is highly experimental and under development, use at your own risk!

## Running

1. Open up the file:

 > TransparentWindow\TransparentWindow\Nancy\Modules\Settings\SettingsModule.cs
 
 At the top of this file is a dictionary of settings, for now there's no config file so edit this to your taste.
 
 2. Run the program
 
## Editing Pages

Pages are served from an in application local webserver. Add new view templates to Nancy/views, the views folder is served at /views so you can access everything in here with the appropriate URL.

## Everything is invisible!

By default all of your web page will be _**completely invisible**_. To change this you need to POST or PUT regions to the server, regions are the area of the form which are visible/clickable.

POST regions to:

 > /screens/regions/{clientid}
 
 in the form (e.g. JSON, but you could probably use XML too):
 
     { X: 0, Y: 0, Width: 0, Height: 0 }
 
 PUT to the same address, but send an array of regions instead:
 
     {
         Regions: [
             { X: 0, Y: 0, Width: 0, Height: 0 }
         ]
     }
     
Now the areas inside a region should be visible and clickable!