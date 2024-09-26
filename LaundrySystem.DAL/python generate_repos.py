import os

# Define the models you want to create interfaces and repositories for
models = [
    'AppUser',
    'Booking',
    'DesiredTimeslot',
    'LostAndFound',
    'Room',
    'Timeslot'
]

# Directories to place the generated files
interfaces_dir = './Repos/Interfaces'
repos_dir = './Repos/Implementations'

# Ensure directories exist
os.makedirs(interfaces_dir, exist_ok=True)
os.makedirs(repos_dir, exist_ok=True)

# Function to create interface content for repositories
def create_interface_content(model):
    return f"""namespace LaundrySystem.DAL.Repos.Interfaces
{{
    using LaundrySystem.DAL.Entities;

    public interface I{model}Repo : IBaseRepo<{model}>
    {{
        // Add any additional methods specific to {model} if needed
    }}
}}
"""

# Function to create repository implementation content
def create_repo_content(model):
    return f"""namespace LaundrySystem.DAL.Repos
{{
    using LaundrySystem.DAL.DataModel;
    using LaundrySystem.DAL.Entities;
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.DAL.Repos.Interfaces;

    public class {model}Repo : BaseRepo<{model}>, I{model}Repo
    {{
        public {model}Repo(DataContext dataContext) : base(dataContext)
        {{
        }}
    }}
}}
"""

# Generate the repository interface and implementation files for each model
for model in models:
    # Interface file
    interface_file_path = os.path.join(interfaces_dir, f'I{model}Repo.cs')
    with open(interface_file_path, 'w') as f:
        f.write(create_interface_content(model))

    # Repository implementation file
    repo_file_path = os.path.join(repos_dir, f'{model}Repo.cs')
    with open(repo_file_path, 'w') as f:
        f.write(create_repo_content(model))

print("Repository interface and implementation files generated successfully.")
