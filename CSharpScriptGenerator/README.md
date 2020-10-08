This editor script allows easier creation of interfaces and classes through `Create --> C# Script`.

### How it works?

1. Create C# script `Right click --> Create --> C# script`
2. Enter desired script name
3. If entered name starts with *I* - script type selection dialog is shown (interface or class). Otherwise class is created.
4. Based on template inside editor script, script content is populated.

### Note

This editor script completely ignores default Unity Editor C# script template.


### Setup

Just paste given editor script inside your project under folder named *Editor* (you might need to create one).  
If this fails, search for Unity Editor scripts, there will be in-depth tutorials how Editor scripts work.

For simple configuration options, check `const` and `static` fields at the top of the script.