using Microsoft.AspNetCore.Identity;

namespace minimalApi.Identidades
{
    public class Usuario
    {
            public Guid idusuario { get; set; }
            public string username { get; set; } = null!;

            public string normalizedUserName {  get; set; }=null!;
            public string email {  get; set; } = null!;
            public string normalizedEmail {  get; set; } = null!;
            public bool emailConfirmed {  get; set; }
            public string passwordHash { get; set; } = null!;
            public string securityStamp {  get; set; } = null!;
            public string concurrencyStamp {  get; set; } = null!;
            public string phoneNumber {  get; set; } = null!;
            public bool phoneNumberConfirmed {  get; set; }
            public bool twoFactorEnabled {  get; set; }
            public DateTime lockoutEnd {  get; set; }
            public bool lockoutEnabled { get; set; }
            public int accessFaildCount {  get; set; }

    }
}
