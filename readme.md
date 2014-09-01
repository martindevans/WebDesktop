# WebDesktop

WebDesktop puts a transparent window over every monitor screen and then draws a webpage onto them, you can use this functionality to build desktop widgets entirely in a web stack.

### Warning

This is highly experimental and under development, use at your own risk!

## Running

1. Open up the file:

 > TransparentWindow\TransparentWindow\Nancy\Modules\Settings\SettingsModule.cs
 
 At the top of this file is a dictionary of settings, for now there's no config file so edit this to your taste.
 
 2. Run the program
 
## Editing Pages

Pages are served from an in application local webserver. Add new view templates to Nancy/views, the views folder is served at /views so you can access everything in here with the appropriate URL.

## I Can't Click On My Web Page!

By default the entire page is set to be *transparent* to mouse clicks - all mouse clicks pass straight through the form and you can interact with your desktop and normal. To change this you need to POST or PUT regions to the server, regions are the area of the form which are clickable.

POST regions to:

 > /screens/regions/{clientid}
 
 in the form (using JSON here, in theory you could use XML too):
 
     { X: 0, Y: 0, Width: 100, Height: 100 }
     
This will add a 100x100 pixel region in the top left of the screen as a clickable region.
 
 PUT to the same address with an array of regions:
 
     {
         Regions: [
             { X: 0, Y: 0, Width: 100, Height: 100 },
             { X: 100, Y: 100, Width: 100, Height: 100 }
         ]
     }
     
This will remove all previous clickable regions (because it's a PUT) and make 2 areas of the screen clickable.