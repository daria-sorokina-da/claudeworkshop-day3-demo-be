# Infrastructure requirements (describe to Claude and ask it to generate the resources below):
#
#  - Look up the existing Resource Group and Windows App Service Plan by name
#    (use data sources, do not create new ones)
#  - Windows App Service (.NET 8) wired to the existing plan
#  - App setting: ASPNETCORE_ENVIRONMENT = Staging
#  - Output the API hostname

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {}
}
