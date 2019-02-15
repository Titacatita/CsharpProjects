[T4Scaffolding.Scaffolder(Description = "Enter a description of Base here")][CmdletBinding()]
param([parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string]$viewName, 
	[string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false
)

$outputPath = "Views\" + $viewName + "View"
$codeBehindOutputPath = "Views\" + $viewName + "View"
$viewModeloutputPath = "ViewModels\" + $viewName + "ViewModel"
$viewModeloutputCompletePath = $viewModeloutputPath + ".cs"
$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value + ".Views"
$viewModelNamespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value + ".ViewModels"

Add-ProjectItemViaTemplate $outputPath -Template CaliburnMicroViewTemplate `
	-Model @{ Namespace = $namespace; ExampleValue = "Hello, world!"; NameOfClass = $viewName + "View" } `
	-SuccessMessage "Added View output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

Add-ProjectItemViaTemplate $codeBehindOutputPath -Template CaliburnMicroViewTemplate.xaml `
-Model @{ Namespace = $namespace; ExampleValue = "Hello, world!"; NameOfClass = $viewName + "View" } `
-SuccessMessage "Added View codebehind at {0}." `
-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

Add-ProjectItemViaTemplate $viewModeloutputPath -Template CaliburnMicroViewModelTemplate `
-Model @{ Namespace = $viewModelNamespace; NameOfClass = $viewName + "ViewModel" } `
-SuccessMessage "Added ViewModel output at {0}" `
-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

$viewModelFile = Get-ProjectItem $viewModeloutputCompletePath
$viewModelFile.Open()
$viewModelFile.Document.Activate()

