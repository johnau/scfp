# Documentation guidelines

## Folder structure

All documentation is stored in the **/docs** folder.

* **general/architecture-decisions** - All architecture decisions are documented here. Everything lands in the [decision log](../architecture-decisions/decision-log.md) in the root of the docs folder. All decisions that need more explanation than a one line will have separate docs here.
* **general/getting-started** - Limited documentation with the purpose of helping new people on the team to get started and setup.
* **general/guidelines** - General guidelines for the project.
* **services** - All documentation on services and their usage - important for developers.

> [!TIP]
> Prevent creating folders in the root level as much as possible.

## Markdownlint

Standard documentation is writting using Markdown files (MD). [Markdownlint](https://github.com/markdownlint/markdownlint) should be used to check for properly structured markdown syntax. This is needed for the generation of static HTML documentation. You can use [Mardownlint extension for Visual Studio code](https://marketplace.visualstudio.com/items?itemName=DavidAnson.vscode-markdownlint) to get these checks and helps while you type in Visual Studio code.

## Use of images and other attachments

If you are writing a document and you have images or other files attached to that document, these extra files must be place in the **/docs/.attachments** folder. This is because of how [DocFX](https://dotnet.github.io/docfx/) is working, which is being used to generate documentation including API documentation from the source code. Also, the `DocLinkChecker` tool uses this location to validate documents versus attachments for orphaned items.
