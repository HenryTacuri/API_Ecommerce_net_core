**¿Qué es el caché en una API?**

Caché es una técnica para almacenar temporalmente respuestas con el fin de mejorar el rendimiento y reducir la carga del servidor.

**.NET Core permite caché en diferentes capas:**

* Caché en memoria (IMemoryCache)

* Caché distribuido en diferentes nodos (IDistributedCache)

* Caché en middleware (Response Caching Middleware)


**Middleware de caché de respuestas**

* .NET Core incluye Response Caching Middleware, que alamacena y reutiliza respuestas HTTP según las cabeceras configuradas.

* Se activa mediante las cabeceras Cache-Control y Vary.

* Ideal para contenido público y estático (no personalizado por usuario).

* No guarda contenido que incluya cookies, solo funciona con estados 200 OK.



El versionamiento de una API se puede poner la URL, los parametros de una URL o en el header.





**¿Por qué utilizar ASP.NET CORE IDENTITY?**

.NET Core identity es el sistema de autenticación y gestion de usuarios incluido en .NET.

Proporciona una solución completa y extensible para menejar:

* Registro e inicio de sesión de usuarios.
* Roles y Claims.
* Cambio de contraseña, recuperación de cuenta, validación por email, autenticación de dos factores, etc.



**Beneficios de usar ASP.NET CORE IDENTITY vs Implementación manual**

| Característica                  | Manual                                   | ASP.NET Core Identity                           |
|---------------------------------|-------------------------------------------|-------------------------------------------------|
| Registro de usuarios            | Implementación personalizada              | Integrado, con validaciones y extensiones        |
| Hash de contraseñas             | Riesgo de errores o uso de paquetes externos | Usa algoritmos seguros como PBKDF2            |
| Roles y Claims                  | Lógica personalizada                      | Soporte integrado                                |
| Validación por email / recuperación | Desarrollo manual                     | Integrado y configurable                         |
| Protección de datos sensibles   | Requiere medidas adicionales              | Cumple con OWASP y buenas prácticas             |


**Diferencias con IDENTITIY SERVER y DUENDE IDENTITY SERVER**

| Característica            | ASP.NET Core Identity                            | Identity Server / Duende Identity Server                      |
|---------------------------|---------------------------------------------------|----------------------------------------------------------------|
| Objetivo principal        | Gestión de usuarios y roles                      | Proveedor de identidad (OAuth2 / OpenID)                      |
| Alcance                   | Local (dentro del mismo proyecto)                | Distribuido (autenticación centralizada)                      |
| ¿Emite tokens JWT?        | No directamente                                   | Sí, como Authorization Server                                 |
| ¿Cuándo usarlo?           | Apps medianas, monolitos o APIs internas         | Apps con múltiples clientes o autenticación federada          |


En identity el password tiene que cumplir reglas, como por ejemplo: Henry1234567*

