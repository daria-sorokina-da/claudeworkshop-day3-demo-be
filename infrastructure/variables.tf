variable "project_name" {
  type        = string
  description = "Prefix used for the App Service name."
  default     = "stable-api"
}

variable "location" {
  type        = string
  description = "Azure region for all resources."
  default     = "uksouth"
}

variable "resource_group_name" {
  type        = string
  description = "Name of the existing Resource Group to deploy into."
}

variable "app_service_plan_name" {
  type        = string
  description = "Name of the existing Windows App Service Plan to use."
}
