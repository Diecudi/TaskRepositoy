# Diagnóstico y Solución - Tareas no Visibles

## Problema
Las tareas no se ven en el tablero ni en el calendario, aunque el frontend está deployado en Render.

## Pasos para Diagnosticar

### 1. Verificar Conexión a Base de Datos
1. Abre tu aplicación en Render
2. Abre **DevTools** (F12) en tu navegador
3. Ve a la pestaña **Console**
4. Verifica que aparezca un mensaje con `Database Health: {status: "OK", ...}`

Si ves error de conexión:
- La cadena de conexión a Aiven no está correctamente configurada en Render

### 2. Verificar Variables de Entorno en Render

En Render, debes tener configurada esta variable de entorno:

```
ConnectionStrings__DefaultConnection = Server=tu-servidor.h.aivencloud.com;Port=20772;Database=defaultdb;User=avnadmin;Password=TU_CONTRASEÑA_REAL;SslMode=Required;
```

#### Cómo Configurarla:
1. Ve a tu proyecto en Render: https://dashboard.render.com
2. Selecciona tu Web Service
3. Haz click en **Settings**
4. Ve a la sección **Environment**
5. Agrega o edita la variable:
   - **Key**: `ConnectionStrings__DefaultConnection`
   - **Value**: (Tu cadena completa de conexión de Aiven)
6. Haz click en **Save**
7. Tu aplicación se redesplegará automáticamente

### 3. Obtener tu Cadena de Conexión de Aiven

Si no la tienes a mano:
1. Ve a https://console.aiven.io
2. Selecciona tu base de datos MySQL
3. Ve a **Service Details**
4. Busca **Connection Information** o **JDBC URL**
5. Copia la cadena en este formato:
   ```
   Server=HOST;Port=PORT;Database=DATABASE;User=USER;Password=PASSWORD;SslMode=Required;
   ```

### 4. Verificar que el Código está Actualizado

Asegúrate de que hayas hecho git push de estos cambios:
- `Program.cs` - Debe leer de ConnectionStrings
- `appsettings.json` - Solo tiene placeholder
- `Controllers/TaskController.cs` - Tiene el endpoint HealthCheck

```bash
cd c:\Users\Daysi\OneDrive\Desktop\Tasks
git add .
git commit -m "Fix: database configuration for Aiven"
git push
```

### 5. Verificar Logs de Render

1. Ve a tu Web Service en Render
2. Ve a la pestaña **Logs**
3. Busca mensajes de error de base de datos
4. Si ves "Connection refused" o "Unknown host", es un problema de variables de entorno

## Verificación Rápida

Una vez configurado, visita esta URL (sin necesidad de login):
```
https://tu-app.render.com/Task/HealthCheck
```

Deberías ver algo como:
```json
{
  "status": "OK",
  "database": "Connected",
  "projects": 5,
  "tasks": 12,
  "users": 1,
  "timestamp": "2026-06-02T15:30:00"
}
```

## Soluciones Comunes

### Error: "Connection timeout"
- Verifica que Aiven está configurado para permitir conexiones desde Render
- En Aiven: **Settings** → **Allowed IP addresses** - Agrega 0.0.0.0/0 (o la IP de Render)

### Error: "Unknown user"
- La contraseña de Aiven es incorrecta en la variable de entorno

### Error: "Unknown database"
- El nombre de la base de datos está mal en la cadena de conexión

### No hay tareas pero el Health Check dice OK
- No hay datos en la base de datos
- Necesitas crear un proyecto primero, luego sprints, luego tareas

## Si Todo Sigue Sin Funcionar

1. Verifica en Render: **Settings** → **Pull Request Previews** - Asegúrate que el auto-deploy está activado
2. Fuerza un redeploy: Ve a **Deploys** y haz click en **Deploy latest commit**
3. Limpia el caché del navegador: Ctrl+Shift+Delete (Windows) / Cmd+Shift+Delete (Mac)
4. Abre en una ventana de incógnito
