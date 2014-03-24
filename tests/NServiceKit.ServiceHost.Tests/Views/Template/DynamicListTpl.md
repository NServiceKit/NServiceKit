# @Model.FirstName Dynamic Markdown Template

Hello @Model.FirstName,

  * @Model.LastName
  * @Model.FirstName

# heading 1

@foreach (var link in Model.Links) {
  - @link.Name - @link.Href
}

## heading 2

This is a [NServiceKit.net link](http://www.NServiceKit.net)

### heading 3

