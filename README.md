# Sistema de Presupuesto Mensual Personal

`Teoria Base de Datos 1: Proyecto Final
Cristina Sabillón - 22351004`

## Descripción
El **Sistema de Presupuesto Mensual Personal** permite a los usuarios planificar, controlar y analizar sus finanzas personales de manera eficiente. Gestiona **ingresos**, **gastos**, **obligaciones financieras** y **metas de ahorro**, generando reportes analíticos para la toma de decisiones.

## Tecnologías utilizadas
- **Base de datos:** Microsoft SQL Server  
- **Gestión de BD:** SQL Server Management Studio  
- **Lenguage de Programacion:** C#
- **Frontend:** Consola

---

## Objetivo
Desarrollar una herramienta que ayude a los usuarios a organizar sus finanzas personales, mejorar el control de gastos y fomentar el ahorro, aplicando buenas prácticas de diseño y gestión de bases de datos.

---

## Alcance del Sistema
El sistema permite:  
- Gestionar usuarios con información de perfil básica.  
- Definir presupuestos con vigencia temporal (mes/año).  
- Clasificar ingresos, gastos y ahorros mediante **categorías** y **subcategorías**.  
- Registrar obligaciones fijas mensuales.  
- Registrar transacciones individuales por subcategoría.  
- Generar reportes analíticos y gráficos de gestión financiera.  

---

## Modelo de Datos
El sistema incluye las siguientes entidades:

- **USUARIO:** Información básica, salario base, estado activo/inactivo.  
- **CATEGORIA:** Clasificación de ingresos, gastos o ahorros, con subcategoría obligatoria por defecto.  
- **SUBCATEGORIA:** Desglose de categorías para control detallado.  
- **PRESUPUESTO:** Plan financiero para un período específico, con totales de ingresos, gastos y ahorro.  
- **PRESUPUESTO_DETALLE:** Distribución mensual del presupuesto por subcategoría.  
- **OBLIGACION_FIJA:** Compromisos financieros recurrentes.  
- **TRANSACCION:** Registro de movimientos financieros reales, con imputación mensual independiente de la fecha real.  

> Todas las tablas incluyen **campos de auditoría**: `creado_por`, `modificado_por`, `creado_en`, `modificado_en`.

---
## Procedimientos Almacenados
| Procedimiento                          | Descripción                                                                                                                             |
| -------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| `sp_crear_presupuesto_completo`        | Crea un presupuesto junto con todas sus subcategorías en una sola transacción, asignando montos mensuales.                              |
| `sp_registrar_transaccion_completa`    | Registra una transacción validando que la fecha y el monto estén dentro de la vigencia del presupuesto y que el año/mes sean correctos. |
| `sp_procesar_obligaciones_mes`         | Revisa todas las obligaciones activas de un usuario y genera alertas para las que vencen en el mes especificado.                        |
| `sp_calcular_balance_mensual`          | Calcula el resumen financiero mensual, incluyendo ingresos, gastos, ahorros y balance final.                                            |
| `sp_calcular_monto_ejecutado_mes`      | Suma todas las transacciones de una subcategoría para un mes determinado.                                                               |
| `sp_calcular_porcentaje_ejecucion_mes` | Calcula el porcentaje de ejecución comparando monto ejecutado versus monto presupuestado.                                               |
| `sp_cerrar_presupuesto`                | Marca un presupuesto como cerrado, valida la fecha de fin y genera un resumen de ejecución.                                             |
| `sp_obtener_resumen_categoria_mes`     | Genera un resumen de una categoría sumando los montos presupuestados y ejecutados de todas sus subcategorías en un mes específico.      |

--

## Funciones

| Función                                    | Descripción                                                                                                  |
| ------------------------------------------ | ------------------------------------------------------------------------------------------------------------ |
| `fn_calcular_monto_ejecutado`              | Retorna el monto total ejecutado de una subcategoría en un mes específico.                                   |
| `fn_calcular_porcentaje_ejecutado`         | Calcula el porcentaje de ejecución de una subcategoría comparando el gasto real con el presupuesto asignado. |
| `fn_obtener_balance_subcategoria`          | Retorna el balance disponible de una subcategoría (presupuesto menos monto ejecutado).                       |
| `fn_obtener_total_categoria_mes`           | Retorna el total presupuestado de todas las subcategorías de una categoría en un mes específico.             |
| `fn_obtener_total_ejecutado_categoria_mes` | Retorna el total ejecutado de una categoría sumando todas sus subcategorías en un mes determinado.           |
| `fn_dias_hasta_vencimiento`                | Calcula los días restantes hasta el vencimiento de una obligación.                                           |
| `fn_validar_vigencia_presupuesto`          | Verifica si una fecha se encuentra dentro del período vigente de un presupuesto.                             |
| `fn_obtener_categoria_por_subcategoria`    | Retorna la categoría padre a la que pertenece una subcategoría.                                              |
| `fn_calcular_proyeccion_gasto_mensual`     | Proyecta el gasto final del mes basado en el comportamiento actual y los días transcurridos.                 |
| `fn_obtener_promedio_gasto_subcategoria`   | Retorna el promedio de gasto de una subcategoría durante los últimos N meses.                                |


---

## Triggers obligatorios
- Creación automática de subcategoría por defecto al insertar una categoría.  

---

## Datos de prueba
- Generación de transacciones y obligaciones reales para **dos meses completos**.  
- Montos coherentes y distribución temporal realista.  
- Balance mensual variado para simular escenarios financieros reales.  

---
## Modulos del Sistema
| Modulo                                     | Funcionalidad                                                    |
| ------------------------------------------ | -----------------------------------------------------------------|
| `Login / Registro`   | Deja al usuario crear o utilizar una cuenta para ingresar al programa                  |
| `Presupuesto`        | CRUD presupuesto y presupuesto detalle + Procedimientos de Logica para los balances    |
| `Obligacion Fija`    | CRUD obligacion + funcion de estar vigente                                             |
| `Transaccion`        | CRUD + procedimiento de transaccion completa                                           |
| `Categorias`         | CRUD categoria y subcategoria                                                          |
| `Reportes`           | 6 reportes para exportar a pdf                                                         |
| `Usuario`            | CRUD usuario para ver y editar usuarios                                                |
| `Menu Principal`     | Menu para poder acceder al resto de menus                                              |

---

## DATOS DE PRUEBA
Para la prueba se utilizo :
`Correo Electronico : mizi.till@email.com`
`Contraseña: 1234`

---

## Estructura del Proyecto

    CAPA DE PRESENTACIÓN (Frontend)
    ↓
    CAPA DE NEGOCIO (Backend/API)
    ↓
    CAPA DE DATOS (Base de Datos)

## Reportes 
El sistema genera 6 reportes obligatorios:  
1. **Resumen Mensual Ingresos vs Gastos vs Ahorros**  
2. **Distribución de Gastos por Categoría**  
3. **Cumplimiento de Presupuesto por Categoría/Subcategoría**  
4. **Tendencia de Gastos por Categoría en el Tiempo**  
5. **Estado de Obligaciones Fijas y Cumplimiento de Pagos**  
6. **Progreso de Metas de Ahorro**  

Todos los reportes se pueden exportar a PDF.
---

## REFLEXION SOBRE EL PROCESO DEL PROYECTO
El desarollo del proyecto me ayudo a poder entender de mejor manera como se realizan las bases de datos al igual de la importancia de tener una base de datos bien estructurada.

---

## Diseño de la Base de Datos
El diseño de la base de datos se puede encontrar en el archivo docs donde se hallara el dbml.

---

## Desafios enfentrados y Soluciones
Los desafios que encontre fue en la realizacion del presupeusto completo con el uso del json, que se me realizo algo dificil saber como tener que manerjarlo, tambien diria que el sintaxis a veces se me hacia dificultad gracias al hecho de que el sintaxis es muy diferente a sintaxis de los lenguajes de programacion a los cuales estaba acostumbrada.

---

## Aprendizaje Clave
EL aprendizaje clave fue el manejo de base de datos, aprendiendo a crear y como utilizar funciones, procedimientos y triggers, al igual que funciones especiales como crosstab, funciones de ventana y la realizacion de validaciones dentro de una base de datos.

---

## Sugerencia de mejoras del Proyecto
Recomendaria mejorar lo visual, en ves de consola talvez utilizar gui y en la parte de reportes mejor utilizar otro paquete para realizar la exportacion y creacion de graficos a pdf.

---

### #NOTA: Se utilizo IA en la parte visual del programa, al igual que en la parte visual de los pdfs y la generacion de datos de prueba.
