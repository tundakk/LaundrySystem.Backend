import os

# Define the models for which you want to create controllers
models = [
    'Address',
    'ChatMessage',
    'Household',
    'LaundryReservation',
    'LostAndFound',
    'ServiceMessage',
    'Slot'
]

# Directory to place the generated controller files
controllers_dir = './Controllers'

# Ensure the directory exists
os.makedirs(controllers_dir, exist_ok=True)

# Function to create controller content
def create_controller_content(model):
    return f"""using LaundrySystem.Api.Controllers.Base;
using LaundrySystem.BLL.Services.Interfaces;
using LaundrySystem.Domain.Model.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace LaundrySystem.API.Controllers
{{
    /// <summary>
    /// {model}sController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class {model}sController : BaseController<{model}sController>
    {{
        private readonly I{model}Service _{model.lower()}Service;

        /// <summary>
        /// Initializes a new instance of the <see cref="{model}sController"/> class.
        /// </summary>
        /// <param name="{model.lower()}Service">The {model} service.</param>
        /// <param name="logger">The logger.</param>
        public {model}sController(I{model}Service {model.lower()}Service, ILogger<{model}sController> logger)
            : base(logger)
        {{
            _{model.lower()}Service = {model.lower()}Service;
        }}

        ///<inheritdoc/>
        [HttpGet]
        public IActionResult GetAll()
        {{
            try
            {{
                var response = _{model.lower()}Service.GetAll();
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
        public IActionResult GetById(int id)
        {{
            try
            {{
                var response = _{model.lower()}Service.GetById(id);
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
        public IActionResult Insert([FromBody] {model}Model {model.lower()}Model)
        {{
            try
            {{
                var response = _{model.lower()}Service.Insert({model.lower()}Model);
                if (!response.Success)
                {{
                    return BadRequest(response.Message);
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
        public IActionResult Update(int id, [FromBody] {model}Model {model.lower()}Model)
        {{
            try
            {{
                {model.lower()}Model.Id = id;
                var response = _{model.lower()}Service.Update({model.lower()}Model);
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
        public IActionResult Delete(int id)
        {{
            try
            {{
                var response = _{model.lower()}Service.Delete(id);
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
