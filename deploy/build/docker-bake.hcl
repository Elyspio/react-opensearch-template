target "default" {
	context    = "../.."
	dockerfile = "./deploy/build/dockerfile"
	platforms  = [
		"linux/amd64",
#		"linux/arm64"
	]
	tags = [
		"elyspio/react-api-template:latest"
	]
	args = {
		SLN_PATH         = "back/ExampleApi.sln"
		MAIN_CSPROJ_PATH = "Web/Example.Api.Web.csproj"
		ROOT_FOLDER      = "back/"
		ENTRY_DLL        = "Example.Api.Web.dll"
	}
}