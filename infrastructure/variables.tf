variable "project_name" {
  type        = string
  description = "Prefix used for all resource names."
  default     = "stable-api"
}

variable "location" {
  type        = string
  description = "Azure region for all resources."
  default     = "uksouth"
}
