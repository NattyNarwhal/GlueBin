# GlueBin

This is a pastebin service, powered by C#/Nancy/MongoDB.

## TODO

* [ ] File uploading?
* [X] Authentication. Classes are set up, but they don't do anything, and the routes aren't set up to require auth.
 * Basic authentication right now.
* [ ] Paste management. This depends on authentication.
* [X] Syntax highlighting. Will need to update the model, run post-processing in the route, then show the rendering.
 * The highlighter is server-side, and perhaps a bit rigid in styling. (CSS mode looks like it requires quite a stylesheet.) It does work though.
* [ ] Searching. Can be pretty simple.
* [ ] Expiry. Manual deletion would be nice, but scheduling a timer job to delete can be even better.