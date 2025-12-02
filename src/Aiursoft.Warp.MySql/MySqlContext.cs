using Aiursoft.Warp.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Warp.MySql;

public class MySqlContext(DbContextOptions<MySqlContext> options) : TemplateDbContext(options);
