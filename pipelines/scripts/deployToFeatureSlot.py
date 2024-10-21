import os
import subprocess
import sys

def get_slot_name(branch_name, custom_slot_name):
    if custom_slot_name != "branch name":
        return custom_slot_name
    return branch_name.replace('/', '-')  # Replace '/' with '-' to make a valid slot name

def check_existing_slots(resource_group, app_name):
    command = f"az webapp deployment slot list --resource-group {resource_group} --name {app_name} --query '[].name' --output tsv"
    result = subprocess.run(command, shell=True, capture_output=True, text=True)
    if result.returncode != 0:
        print("Error retrieving existing slots:", result.stderr)
        sys.exit(1)
    return result.stdout.strip().splitlines()

def create_slot(resource_group, app_name, slot_name, json_file_path):
    # Create the deployment slot
    command = f"az webapp deployment slot create --resource-group {resource_group} --name {app_name} --slot {slot_name}"
    result = subprocess.run(command, shell=True, capture_output=True, text=True)
    if result.returncode != 0:
        print("Error creating slot:", result.stderr)
        sys.exit(1)
    print(f"Slot '{slot_name}' created successfully.")

     # Set the environment variables directly from the JSON file
    command_set = f"az webapp config appsettings set --resource-group {resource_group} --name {app_name} --slot {slot_name} --settings @{json_file_path}"
    result_set = subprocess.run(command_set, shell=True, capture_output=True, text=True)
    if result_set.returncode != 0:
        print("Error setting environment variables:", result_set.stderr)
        sys.exit(1)
    print(f"Environment variables set for slot '{slot_name}' from '{json_file_path}' successfully.")

def deploy_to_slot(resource_group, app_name, slot_name, deployment_package):
    command = f"az webapp deployment source config-zip --resource-group {resource_group} --name {app_name} --slot {slot_name} --src {deployment_package}"
    result = subprocess.run(command, shell=True, capture_output=True, text=True)
    if result.returncode != 0:
        print("Error deploying to slot:", result.stderr)
        sys.exit(1)
    print(f"Deployment to slot '{slot_name}' completed successfully.")

if __name__ == "__main__":
    # Read environment variables
    deploy_to_feature_slot = os.getenv("DEPLOY_TO_FEATURE_SLOT", "false").lower() == "true"  # Convert to boolean

    # Check if deployment to the feature slot is enabled
    if not deploy_to_feature_slot:
        print("Deployment to feature slot is skipped.")
        sys.exit(0)  # Exit gracefully if deployment is not needed

    # If we reach here, it means deploy_to_feature_slot is True
    resource_group = os.getenv("RESOURCE_GROUP")
    app_name = os.getenv("APP_NAME")
    custom_slot_name = os.getenv("CUSTOM_SLOT_NAME", "")
    branch_name = os.getenv("BUILD_SOURCEBRANCHNAME", "")
    deployment_package = os.getenv("DEPLOYMENT_PACKAGE")

    slot_name = get_slot_name(branch_name, custom_slot_name)
    print(f"Using slot name: {slot_name}")

    existing_slots = check_existing_slots(resource_group, app_name)
    print(f"Existing slots: {existing_slots}")

    if slot_name in existing_slots:
        print(f"Slot '{slot_name}' already exists. Reusing the existing slot.")
    else:
        max_slots = 5  # Change this to your actual limit based on pricing tier
        if len(existing_slots) >= max_slots:
            print(f"Error: Maximum number of slots ({max_slots}) has been reached.")
            sys.exit(1)
        create_slot(resource_group, app_name, slot_name, "appsettings.Staging.json")

    deploy_to_slot(resource_group, app_name, slot_name, deployment_package)
