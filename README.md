# Sistema de Presupuesto Mensual Personal

## Descripción
El **Sistema de Presupuesto Mensual Personal** permite a los usuarios planificar, controlar y analizar sus finanzas personales de manera eficiente. Gestiona **ingresos**, **gastos**, **obligaciones financieras** y **metas de ahorro**, generando reportes analíticos para la toma de decisiones.

---

## Tecnologías utilizadas
- **Base de datos:** Microsoft SQL Server  
- **Gestión de BD:** SQL Server Management Studio  
- **Backend:** Python / API REST (con procedimientos almacenados)  
- **Frontend:** Web (HTML, CSS, JavaScript)  
- **Reportería:** Metabase  

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
Se implementaron procedimientos para:  
- CRUD completo de todas las tablas.  
- Lógica de negocio: creación de presupuestos completos, registro de transacciones, cálculo de balances mensuales, cierre de presupuestos y resúmenes por categoría.  

---

## Funciones
- Cálculo de monto ejecutado por subcategoría y categoría.  
- Porcentaje de ejecución vs presupuesto.  
- Balance mensual por subcategoría.  
- Días restantes hasta el vencimiento de obligaciones.  

---

## Triggers obligatorios
- Creación automática de subcategoría por defecto al insertar una categoría.  

---

## Datos de prueba
- Generación de transacciones y obligaciones reales para **dos meses completos**.  
- Montos coherentes y distribución temporal realista.  
- Balance mensual variado para simular escenarios financieros reales.  

---

## Estructura del Proyecto

    CAPA DE PRESENTACIÓN (Frontend)
    ↓
    CAPA DE NEGOCIO (Backend/API)
    ↓
    CAPA DE DATOS (Base de Datos)

## Reportes (Metabase)
El sistema genera 6 reportes obligatorios:  
1. **Resumen Mensual Ingresos vs Gastos vs Ahorros**  
2. **Distribución de Gastos por Categoría**  
3. **Cumplimiento de Presupuesto por Categoría/Subcategoría**  
4. **Tendencia de Gastos por Categoría en el Tiempo**  
5. **Estado de Obligaciones Fijas y Cumplimiento de Pagos**  
6. **Progreso de Metas de Ahorro**  

Todos los reportes se pueden exportar a PDF.

## Reportes (Metabase)
El sistema genera 6 reportes obligatorios:  
1. **Resumen Mensual Ingresos vs Gastos vs Ahorros**  
2. **Distribución de Gastos por Categoría**  
3. **Cumplimiento de Presupuesto por Categoría/Subcategoría**  
4. **Tendencia de Gastos por Categoría en el Tiempo**  
5. **Estado de Obligaciones Fijas y Cumplimiento de Pagos**  
6. **Progreso de Metas de Ahorro**  

Todos los reportes se pueden exportar a PDF.

## Evaluación
- Modelo de datos y diseño: 30%  
- Base de datos: 30%  
- Backend (API): 15%  
- Reportería: 10%  
- Frontend: 10%  
- Documentación y presentación: 5%  
