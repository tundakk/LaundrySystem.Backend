import os

# Define the models for which you want to create controllers
models = [
    'AppUser',
    'Booking',
    'DesiredTimeslot',
    'Room',
    'Timeslot',
    'ServiceMessage',
    'LostAndFound'
]

# Directory to place the generated controller files (inside Implementations folder)
controllers_dir = './Controllers/Implementations'

# Ensure the directory exists
os.makedirs(controllers_dir, exist_ok=True)

# Function to create controller content
def create_controller_content(model):
    return f"""using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Services.Interfaces;
using LaundrySystem.BLL.Infrastructure.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LaundrySystem.API.Controllers.Implementations
{{
    /// <summary>
    /// {model}sController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class {model}sController : BaseController<{model}sController>
    {{
        private readonly I{model}Service _{model[0].lower() + model[1:]}Service;

        /// <summary>
        /// Initializes a new instance of the <see cref="{model}sController"/> class.
        /// </summary>
        /// <param name="{model[0].lower() + model[1:]}Service">The {model} service.</param>
        /// <param name="logger">The logger.</param>
        public {model}sController(I{model}Service {model[0].lower() + model[1:]}Service, ILogger<{model}sController> logger)
            : base(logger)
        {{
            _{model[0].lower() + model[1:]}Service = {model[0].lower() + model[1:]}Service;
        }}

        ///<inheritdoc/>
        [HttpGet]
        public IActionResult GetAll()
        {{
            try
            {{
                var response = _{model[0].lower() + model[1:]}Service.GetAll();
                if (!response.Success)
                {{
                    return BadRequest(response.Message);
                }}
                return Ok(response.Data);
            }}
            catch (Exception ex)
            {{
                return HandleError(ex);
            }}
        }}
        
        ///<inheritdoc/>
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {{
            try
            {{
                var response = _{model[0].lower() + model[1:]}Service.GetById(id);
                if (!response.Success)
                {{
                    return BadRequest(response.Message);
                }}
                return Ok(response.Data);
            }}
            catch (Exception ex)
            {{
                return HandleError(ex);
            }}
        }}

        ///<inheritdoc/>
        [HttpPost]
        public IActionResult Insert([FromBody] {model}Model {model[0].lower() + model[1:]}Model)
        {{
            try
            {{
                var response = _{model[0].lower() + model[1:]}Service.Insert({model[0].lower() + model[1:]}Model);
                if (!response.Success)
                {{
                    return BadRequest(response.Message);
                }}
                if (response.Data == null)
                {{
                    return BadRequest("Failed to create the {model}");
                }}
                return CreatedAtAction(nameof(GetById), new {{ id = response.Data.Id }}, response.Data);
            }}
            catch (Exception ex)
            {{
                return HandleError(ex);
            }}
        }}
        
        ///<inheritdoc/>
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] {model}Model {model[0].lower() + model[1:]}Model)
        {{
            try
            {{
                {model[0].lower() + model[1:]}Model.Id = id;
                var response = _{model[0].lower() + model[1:]}Service.Update({model[0].lower() + model[1:]}Model);
                if (!response.Success)
                {{
                    return BadRequest(response.Message);
                }}
                return Ok(response.Data);
            }}
            catch (Exception ex)
            {{
                return HandleError(ex);
            }}
        }}

        ///<inheritdoc/>
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {{
            try
            {{
                var response = _{model[0].lower() + model[1:]}Service.Delete(id);
                if (!response.Success)
                {{
                    return BadRequest(response.Message);
                }}
                return Ok(response.Data);
            }}
            catch (Exception ex)
            {{
                return HandleError(ex);
            }}
        }}
    }}
}}
"""

# Generate the controller files for each model
for model in models:
    controller_file_path = os.path.join(controllers_dir, f'{model}sController.cs')
    with open(controller_file_path, 'w') as f:
        f.write(create_controller_content(model))

print("Controller files generated successfully.")
