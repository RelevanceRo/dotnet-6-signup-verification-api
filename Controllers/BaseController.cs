namespace TCIG.MHUSIGTASAPI.Controllers;

using Microsoft.AspNetCore.Mvc;

using TCIG.MHUSIGTASAPI.Entities;

[Controller]
public abstract class BaseController : ControllerBase
{
    // returns the current authenticated account (null if not logged in)
    public Account Account => (Account)HttpContext.Items["Account"];
}