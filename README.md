# SqlGen

Generate code from SQL Server tables - generator stored procs, table types, C# code

Built in two parts:
1. .NET library that does the code generation
2. .NET (WinForms) application that supports interactive code generation with *drag and drop* of the generated code

![screen shot](https://github.com/busterwood/SqlGen/blob/master/screen-shot.png)

### Server menu

This lists the SQL Server connections configured for the application.

You can add your own connection strings to the application by editing the `SqlGenUi.exe.config` file

### Database menu

Once connected to a server you can change the database via the `Database` menu

### Schema menu

You can filter the table list to only show tables in a specific schema via the `Schema` menu

### Options menu

These options control code generation:
* `Add Grant` to generate `GRANT` statements to a role
* `ALTER stored procs` to generate `ALTER PROC` statements instead of `CREATE PROC`
* `Include Audit columns` to include or exclude any columns starting with `AUDIT_` from the code generators

### Keys list

Some code generators support generation for a specific key, for example:

* `Proc Get` code generator will create a proc to load rows for a specific key
* `Proc Get by list` code generator will create a proc to load rows for a list of keys
* `Table Merge` code generator will add a delete clause to remove rows matching the FK that do not match the supplied data, that is to say the proc call becomes "replace all the rows for the supplied key".

### Multiple selection

You can select multiple items on the tables, keys and code generators lists to create larger scripts.
For example you may wish to:

* create C# classes for multiple tables by selecting multiple tables and the `C# Class` code generator
* create multiple insert, update and delete procs by selecting a table and multiple code generators

### Generated code

This window shows the generated code.  You can drag & drop the code to a text editor, e.g. Visual Studio or Sql Server Management Studio.
