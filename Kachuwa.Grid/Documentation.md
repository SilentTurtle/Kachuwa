# Kachuwa Grid N FormBuilder

[![Kachuwa](http://www.silentturtle.com/wwwroot/resources/img/logo.png)](https://silentturtle.com/products/kachuwa)

K-grid is simple grid builder system to reduce your crud app creation time.

  - Simple and yet powerfull.
  - Allow custom culumn templating with Mustache
  - Allow custom command buttons

### Installation

K-Grid requires [AspNetCore](https://www.microsoft.com/net/core/)  to run.
Clone or Download the source code.
Start Project reference to your project.


### Documentation

@using Kachuwa.KGrid
@using Microsoft.CodeAnalysis
@model IEnumerable<Tixalaya.Model.Theatre>
<link href="~/resources/fontawesome/css/font-awesome.css" rel="stylesheet" />
.
@(Html
    .CreateKachuwaGrid(Model)
    .Build(columns =>
    {
        columns.Add(model => model.Name+model.Description).SetTitle("Name").Encoding(false).Template("<strong>{0}</strong>");
        columns.Add(model => new{model.Name,                        model.Description}).SetTitle("testformat").Encoding(false)
        .Template("<p>{{Name}}</p><p>{{Description}}</p>");
        columns.Add(model => model.Description).SetTitle("Age");
        columns.Add(model => model.AddedOn).SetTitle("AddedOn date");
        columns.Add(model => model.AddedBy).SetTitle("AddedBy");
    }).AddCommands(commands =>
    {
        commands.Add("Edit", "Edit", "alert(1);","fa fa-pencil");
        commands.Add("Delete", "Delete", "alert(1);", "fa fa-trash");

    }).Css("table-hover").RowCss(model => model.IsActive == true ? "active" : "")
    .Empty("No Records Found")
    .Pagination(pager =>
    {
        pager.CurrentPage = 5;//later will come form view bag where page logic implemented
        pager.Api = "asdf/dfd/";
    })

)
.

### Development

Want to contribute? Great!



### Todos

 - Write MOAR Tests
 - Add Night Mode

License
----

MIT


