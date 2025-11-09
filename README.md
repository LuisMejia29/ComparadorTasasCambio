# comparador de tasas
Sistema que compara tasas de cambio de 3 APIs diferentes y te dice cuál da mas dinero.

#que hace este programa?
Imagina que quieres cambiar 100 dolares a peso, este programa pregunta a 3 lugares diferentes cuantos peso te dan y dice cual conviene mas.
Lo hace todo al mismo tiempo 'en paralelo' para que mas rapido.

#como funciona
1. Se le indica cuanto dinero quieres cambiar
2. Pregunta a 3 apis diferentes
3. Compara las respuestas
4. Te dice cual oferta es mejor

#funciona aunque varia apis fallen.

#requisitos para usar el programa
- .net 9.0 
- O Docker pueden usar docker que mas facil 

## como se ejecuta?

### con .net 'necesitas tener .net'
```bash
# Descargar el proyecto
git clone al repositorio
cd ComparadorTasasCambio

# Compilar
dotnet build

# Ejecutar con valores por defecto (100 Usd a Dop)
dotnet run --project src/ComparadorTasasCambio.App/ComparadorTasasCambio.App.csproj

# Ejecutar con tus propios valores
dotnet run --project src/ComparadorTasasCambio.App/ComparadorTasasCambio.App.csproj -- USD DOP 500


# usando docker
# Verificar que docker este arriba

#docker-compose up
y luego docker run comparadortasascambio-comparador USD DOP 700
```

# usando docker
## Verificar que docker este arriba
```bash
# Construir la imagen
docker build -t comparador-tasas .

# ejecutar la imagen
docker run comparador-tasas

# ejecutar con parametros personalizados
docker run comparador-tasas USD EUR 200 o en DOP

# O con docker-compose (mas facil)
docker-compose up


```

### Probar la imagen ya construida
Si alguien mas construyo la imagen y quieres probarla:
```bash
# descargar la imagen (si esta en un registro)
docker pull usuario/comparador-tasas:latest

# ejecutar
docker run usuario/comparador-tasas:latest

# ver los logs mientras corre
docker run -it usuario/comparador-tasas:latest
```


## ejecutar las pruebas
```bash
dotnet test
```
#hay 15 pruebas que verifican que todo funciona bien.

## Ejemplo de lo que verás
```
comparador de tasa

conversion: 100 Usd a Dop
APIs consultadas: 3
  Exitosas: 2
  Fallidas: 1

Mejor oferta:
   API: API2_XML
   Recibirás: 6,450 Dop
   Tasa: 64.50

Detalles:
  API1_JSON      -> 6,422 Dop (100ms)
  API2_XML       -> 6,450 Dop (150ms)
  API3_ANIDADO   -> ERROR
```

## Cómo está organizado el codigo
- dominio: donde se puso la regla de negocio
- aplicacion: aqui estan los archivos que se usan para la comparacionm
- infraestructura: aqui como se comunican con apis externas

archivo de configuracion `appsettings.json`:
```json
{
  "API1_URL": "https://api1.ejemplo.com",
  "API1_KEY": "clave",
  "API1_TIMEOUT": "5"
}
```

## las 3 apis del la prueba
API1-recibe y envia json normal
API2-recibe y envia XML
API3-recibe y envía json anidadO

cada una tiene su propio formato mas el programa se comunica con las 3 .

## Por que está hecho así
1-consultas en paralelo - consume las 3 apis al mismo tiempo en vez de una a una.
2-si una no responde, no importa sigue con las otras 2.
3-Hay 15 pruebas automáticas que verifican que todo funciona.

## tecnologias usadas 
'No se especifico asi que decidir usar .net ya que hice un curso recientemente pueden visualizarlo en mi linkdln si gusta (https://www.linkedin.com/in/luis-oscar-mej%C3%ADa-fern%C3%A1ndez-3898771b2/)'

- .NET 9.0
- C# 12
- xUnit
- Docker

## nota
todo es una esquema de laboratorio. los test son api simuladas.

Para probar rápido:
```bash
dotnet test
dotnet run --project src/ComparadorTasasCambio.App/ComparadorTasasCambio.App.csproj
```

O con docker:
```bash
docker-compose up
```

## si tienes problemas con docker
- asegurate que docker este corriendo
- si no funciona el build intenta: `docker build --no-cache -t comparador-tasas .`
- verifica que el Dockerfile este en la raiz del proyecto