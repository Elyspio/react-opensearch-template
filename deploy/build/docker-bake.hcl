target "default" {
	context    = "../.."
	dockerfile = "./deploy/build/dockerfile"
	platforms  = [
		"linux/amd64",
	]
	tags = [
		"elyspio/react-opensearch-template:latest"
	]
	args = {
		MAIN_CSPROJ_PATH = "Web/OpenSearch.Api.Web.csproj"
		ROOT_FOLDER      = "back/"
		ENTRY_DLL        = "OpenSearch.Api.Web.dll"
	}
}