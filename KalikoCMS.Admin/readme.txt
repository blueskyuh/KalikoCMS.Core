
Thanks for installing Kaliko CMS version 1.2.5!

Notice if you're upgrading Kaliko CMS (this doesn't apply to new projects):
============================================================================
 If you're upgrading from a previous version prior to version 1.2.1 and are 
 using either composite or collection properties it's recommended to backup 
 your existing database. In version 1.2.1 the third party JSON library was 
 replaced for security reasons. Although the replacement has been fully 
 tested we recommend as an extra precaution to backup the database.
============================================================================

### New in 1.2.5
* Fixed problem with file option in link property
* Fixed problem with omitted content type when returning MVC action result
* Fixed problem with paths starting with "./" for images in HTML editor
* Fixed problem with sort settings not being applied for site root in admin
* Fixed problem with reopening image and link dialogs in admin
* Updated jsTree to a version that not relies on registerElement which will be deprecated in Chrome
* Validate password before trying to change when editing a user in admin
* Added optional configuration setting (enableSessions) to support the use of session state in MVC

New in 1.2.4
* Fixed problem with unicode characters in composition properties
* Fixed problem with default values for menu and sitemap visibility not honored by administration UI
* Fixed problem with duplicate ApplicationPaths when creating a new page running site in virtual folder
* Fixed problem with incompatible data types for some data providers such as Oracle
* Fixed problem with using required flag on fields in composite and collection properties
* Fixed problem with hidden publish and save buttons on iOS
* Fixed problem with freezing page tree when moving pages in admin
* Fixed problem with editor for file properties inside collections
* Added option to specify connection string name in configuration
* Removed client file size limit on file upload
* Added support for running the CMS headless through new content Api

New in 1.2.3
* Fixed script reference problem for property editors in Chrome
* Fixed problem with file property editor not showing correct filename on postback
* Added HtmlHelper extensions for getting URL to startpage or specified page [PR #130]

New in 1.2.2
* Fixed problem with custom property types only used in site definition not registering

New in 1.2.1:
* Replaced JSON component
* Method for setting default values for pages
* Fixed problem where collection property editor breaks on larger property value
* Fixed problem with non-responsive dialog when moving pages
* Fixed problem with image editor in collection properties
* Fixed problem with using composite properties in collections
* Fixed problem with failing page saves on PageLink properties
* Fixed problem with property type scripts when using collection properties
* Fixed problem with preview when site is set up as a subsite
* Fixed problem with short url in editor when site is set up as a subsite
* Fixed problem with editor allowing start publish date after stop publish date
* Added IoC support for MVC controllers
* Added option to select what fields to search in
* Added PageMoved event
* Updated core project to ASP.NET 4.5
* Updated AutoMapper

---

For documentation:
http://kaliko.com/cms/get-started/

For feature requests and bug reports:
https://github.com/KalikoCMS/KalikoCMS.Core/issues

For general support:
http://kaliko.com/cms/forum
