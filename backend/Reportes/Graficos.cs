using PuppeteerSharp;
using SistemaBancario.Database;
using SistemaBancario.Models;
using Microsoft.Data.SqlClient;
using PuppeteerSharp.Media;
using SistemaBancario.Repositories;
using SistemaBancario.Menus;

namespace SistemaBancario
{
    public class Graficos
    {
         private static async Task<string> GenerarPdf(string html, string nombreArchivo)
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            await using var page    = await browser.NewPageAsync();
            await page.SetContentAsync(html, new NavigationOptions
            {
                WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }
            });
            string ruta = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.Desktop), nombreArchivo);
            await page.PdfAsync(ruta, new PdfOptions
            {
                Format          = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions   = new MarginOptions
                {
                    Top = "20mm", Bottom = "20mm", Left = "15mm", Right = "15mm"
                }
            });
            await Task.Delay(300); 
            return ruta;
        }

         private static decimal EjecutarFuncionDecimal(string sql, Action<SqlCommand> parametros)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            parametros(cmd);
            var result = cmd.ExecuteScalar();
            return result == DBNull.Value || result == null ? 0 : Convert.ToDecimal(result);
        }

        private static int EjecutarFuncionInt(string sql, Action<SqlCommand> parametros)
        {
            using var conn = Conexion.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            parametros(cmd);
            var result = cmd.ExecuteScalar();
            return result == DBNull.Value || result == null ? 0 : Convert.ToInt32(result);
        }

         private static string CssBase => @"
            body { font-family: Arial; padding: 30px; color: #333; }
            h1   { font-size: 20px; border-bottom: 2px solid #333; padding-bottom: 8px; }
            .meta { font-size: 12px; color: #666; margin-bottom: 20px; }
            table { width: 100%; border-collapse: collapse; font-size: 13px; margin-top: 16px; }
            th { background: #f5f5f5; padding: 8px; text-align: left; border: 1px solid #ddd; }
            td { padding: 8px; border: 1px solid #ddd; }
            tr:nth-child(even) { background: #fafafa; }
            .cards { display: flex; gap: 16px; margin-bottom: 24px; }
            .card  { flex: 1; border: 1px solid #ddd; border-radius: 8px; padding: 16px; text-align: center; }
            .card .label { font-size: 12px; color: #666; margin-bottom: 8px; }
            .card .value { font-size: 22px; font-weight: bold; }
            .green  { color: #2e7d32; } .red  { color: #c62828; }
            .blue   { color: #1565c0; } .orange { color: #e65100; }
            .footer { margin-top: 40px; font-size: 11px; color: #999; text-align: center; border-top: 1px solid #ddd; padding-top: 12px; }";

        private static string Footer =>
            $"<p class='footer'>Sistema de Presupuesto Personal · {Session.Nombre} {Session.Apellido} · Generado: {DateTime.Now:dd/MM/yyyy HH:mm}</p>";

        // ── Helper ExtraerBody ────────────────────────────────────
        private static string ExtraerBody(string html)
        {
            int inicio = html.IndexOf("<body>") + 6;
            int fin    = html.IndexOf("</body>");
            return inicio > 5 && fin > inicio ? html[inicio..fin] : html;
        }

        // ══════════════════════════════════════════════════════════
        // REPORTE 1
        // ══════════════════════════════════════════════════════════
        private static async Task<string> ObtenerHtmlReporte1(int idPresupuesto, int anio, int mes)
        {
            decimal ingresos = 0, gastos = 0, ahorros = 0, balance = 0;
            using (var conn = Conexion.GetConnection())
            {
                conn.Open();
                using var cmd = new SqlCommand("sp_calcular_balance_mensual", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_id_usuario", Session.IdUsuario);
                cmd.Parameters.AddWithValue("@p_id_presupuesto", idPresupuesto);
                cmd.Parameters.AddWithValue("@p_anio", anio);
                cmd.Parameters.AddWithValue("@p_mes", mes);
                var pI = cmd.Parameters.Add("@p_total_ingresos", System.Data.SqlDbType.Decimal); pI.Direction = System.Data.ParameterDirection.Output;
                var pG = cmd.Parameters.Add("@p_total_gastos", System.Data.SqlDbType.Decimal); pG.Direction = System.Data.ParameterDirection.Output;
                var pA = cmd.Parameters.Add("@p_total_ahorros", System.Data.SqlDbType.Decimal); pA.Direction = System.Data.ParameterDirection.Output;
                var pB = cmd.Parameters.Add("@p_balance_final", System.Data.SqlDbType.Decimal); pB.Direction = System.Data.ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                ingresos = pI.Value == DBNull.Value ? 0 : (decimal)pI.Value;
                gastos   = pG.Value == DBNull.Value ? 0 : (decimal)pG.Value;
                ahorros  = pA.Value == DBNull.Value ? 0 : (decimal)pA.Value;
                balance  = pB.Value == DBNull.Value ? 0 : (decimal)pB.Value;
            }
            string ingStr = ingresos.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            string gasStr = gastos.ToString("F2",   System.Globalization.CultureInfo.InvariantCulture);
            string ahoStr = ahorros.ToString("F2",  System.Globalization.CultureInfo.InvariantCulture);
            string balStr = balance.ToString("F2",  System.Globalization.CultureInfo.InvariantCulture);

            return $@"<!DOCTYPE html><html><head><style>{CssBase}</style></head><body>
                <h1>Reporte 1 — Ingresos vs Gastos vs Ahorros</h1>
                <p class='meta'>Período: {mes}/{anio}</p>
                <div class='cards'>
                    <div class='card'><div class='label'>Ingresos</div><div class='value green'>L. {ingresos:N2}</div></div>
                    <div class='card'><div class='label'>Gastos</div><div class='value red'>L. {gastos:N2}</div></div>
                    <div class='card'><div class='label'>Ahorros</div><div class='value blue'>L. {ahorros:N2}</div></div>
                    <div class='card'><div class='label'>Balance</div><div class='value'>L. {balance:N2}</div></div>
                </div>
                <canvas id='chart1' height='120'></canvas>
                <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
                <script>
                new Chart(document.getElementById('chart1'), {{
                    type: 'bar',
                    data: {{
                        labels: ['{mes}/{anio}'],
                        datasets: [
                            {{ label: 'Ingresos', data: [{ingStr}], backgroundColor: '#4caf50' }},
                            {{ label: 'Gastos',   data: [{gasStr}], backgroundColor: '#f44336' }},
                            {{ label: 'Ahorros',  data: [{ahoStr}], backgroundColor: '#2196f3' }},
                            {{ label: 'Balance',  data: [{balStr}], backgroundColor: '#ff9800' }}
                        ]
                    }},
                    options: {{ responsive: true, plugins: {{ legend: {{ position: 'bottom' }} }}, scales: {{ y: {{ beginAtZero: true }} }} }}
                }});
                </script>
                {Footer}
            </body></html>";
        }

        public static async Task Reporte1(int idPresupuesto, int anio, int mes)
        {
            string ruta = await GenerarPdf(await ObtenerHtmlReporte1(idPresupuesto, anio, mes), $"Reporte1_{mes}_{anio}.pdf");
            Console.WriteLine($"  ✓ Reporte exportado: {ruta}");
            
        }

        public static async Task Reporte2(int anio, int mes)
        {
            string ruta = await GenerarPdf(await ObtenerHtmlReporte2(anio, mes), $"Reporte2_{mes}_{anio}.pdf");
            Console.WriteLine($"  ✓ Reporte exportado: {ruta}");
        }

        public static async Task Reporte3(int idPresupuesto, int anio, int mes)
        {
            string ruta = await GenerarPdf(await ObtenerHtmlReporte3(idPresupuesto, anio, mes), $"Reporte3_{mes}_{anio}.pdf");
            Console.WriteLine($"  ✓ Reporte exportado: {ruta}");
        }
        public static async Task Reporte4(int anioDesde, int mesDesde, int anioHasta, int mesHasta)
        {
            string ruta = await GenerarPdf(await ObtenerHtmlReporte4(anioDesde, mesDesde, anioHasta, mesHasta), $"Reporte4_{mesDesde}{anioDesde}_{mesHasta}{anioHasta}.pdf");
            Console.WriteLine($"  ✓ Reporte exportado: {ruta}");
        }

        // ══════════════════════════════════════════════════════════
        // REPORTE 5
        // ══════════════════════════════════════════════════════════

        public static async Task Reporte5(int anio, int mes)
        {
            string ruta = await GenerarPdf(await ObtenerHtmlReporte5(anio, mes), $"Reporte5_{mes}_{anio}.pdf");
            Console.WriteLine($"  ✓ Reporte exportado: {ruta}");
        }

        // ══════════════════════════════════════════════════════════
        // REPORTE 6
        // ══════════════════════════════════════════════════════════

        public static async Task Reporte6(int idPresupuesto, int anio, int mes)
        {
            string ruta = await GenerarPdf(await ObtenerHtmlReporte6(idPresupuesto, anio, mes), $"Reporte6_{mes}_{anio}.pdf");
            Console.WriteLine($"  ✓ Reporte exportado: {ruta}");
        }

        // ══════════════════════════════════════════════════════════
        // GENERAR TODOS
        // ══════════════════════════════════════════════════════════
        public static async Task GenerarTodos()
        {
            var presupuestos = PresupuestoRepo.Listar();
            var activo = presupuestos.FirstOrDefault(p => p.EstadoPresupuesto == 1);
            if (activo == null) { UI.Error("No tienes un presupuesto activo."); return; }

            int anio = DateTime.Now.Year;
            int mes = DateTime.Now.Month;
            int idP = activo.IdPresupuesto;
            int anioIni = activo.AnioInicio;
            int mesIni  = activo.MesInicio;

            UI.Info($"Generando reportes para {mes}/{anio} — {activo.NombreDescriptivo}...");

            string h1 = await ObtenerHtmlReporte1(idP, anio, mes);
            string h2 = await ObtenerHtmlReporte2(anio, mes);
            string h3 = await ObtenerHtmlReporte3(idP, anio, mes);
            string h4 = await ObtenerHtmlReporte4(anioIni, mesIni, anio, mes);
            string h5 = await ObtenerHtmlReporte5(anio, mes);
            string h6 = await ObtenerHtmlReporte6(idP, anio, mes);

            string htmlCompleto = $@"<!DOCTYPE html><html><head>
                <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
                <style>{CssBase} .page-break {{ page-break-after: always; }}</style>
            </head><body>
                <div class='page-break'>{ExtraerBody(h1)}</div>
                <div class='page-break'>{ExtraerBody(h2)}</div>
                <div class='page-break'>{ExtraerBody(h3)}</div>
                <div class='page-break'>{ExtraerBody(h4)}</div>
                <div class='page-break'>{ExtraerBody(h5)}</div>
                <div>{ExtraerBody(h6)}</div>
            </body></html>";

            string ruta = await GenerarPdf(htmlCompleto, $"Reportes_Completos_{mes}_{anio}.pdf");
            Console.WriteLine($"  ✓ Todos los reportes exportados: {ruta}");
        }
 
private static async Task<string> ObtenerHtmlReporte2(int anio, int mes)
{
    var categorias = new List<(int id, string nombre, decimal total, int transacciones)>();
    using (var conn = Conexion.GetConnection())
    {
        conn.Open();
        using var cmd = new SqlCommand("sp_reporte_distribucion_gastos", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@p_anio", anio);
        cmd.Parameters.AddWithValue("@p_mes",  mes);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            categorias.Add((r.GetInt32(0), r.GetString(1), r.GetDecimal(2), r.GetInt32(3)));
    }
    decimal totalGasto = categorias.Sum(c => c.total);
    string[] colores = { "#f44336","#e91e63","#9c27b0","#3f51b5","#2196f3","#00bcd4","#4caf50","#ff9800","#795548" };
    string labelsJs = string.Join(",", categorias.Select(c => $"'{c.nombre}'"));
    string datosJs = string.Join(",", categorias.Select(c => c.total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)));
    string coloresJs = string.Join(",", categorias.Select((c, i) => $"'{colores[i % colores.Length]}'"));
    var filas = new System.Text.StringBuilder();
    foreach (var c in categorias)
    {
        double pct = totalGasto > 0 ? (double)(c.total / totalGasto) * 100 : 0;
        filas.Append($"<tr><td>{c.nombre}</td><td>L. {c.total:N2}</td><td>{pct:F1}%</td><td>{c.transacciones}</td></tr>");
    }
    return $@"<!DOCTYPE html><html><head><style>{CssBase}
        .layout {{ display: flex; gap: 30px; align-items: flex-start; }}
        .pie    {{ flex: 1; max-width: 350px; }} .tabla {{ flex: 2; }}
    </style></head><body>
        <h1>Reporte 2 — Distribución de Gastos por Categoría</h1>
        <p class='meta'>Período: {mes}/{anio} · Total: L. {totalGasto:N2}</p>
        <div class='layout'>
            <div class='pie'><canvas id='chart2'></canvas></div>
            <div class='tabla'><table>
                <tr><th>Categoría</th><th>Total</th><th>%</th><th>Transacciones</th></tr>
                {filas}
            </table></div>
        </div>
        <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
        <script>
        new Chart(document.getElementById('chart2'), {{
            type: 'doughnut',
            data: {{ labels: [{labelsJs}], datasets: [{{ data: [{datosJs}], backgroundColor: [{coloresJs}] }}] }},
            options: {{ plugins: {{ legend: {{ position: 'bottom' }} }} }}
        }});
        </script>
        {Footer}
    </body></html>";
}
 
private static async Task<string> ObtenerHtmlReporte3(int idPresupuesto, int anio, int mes)
{
    var filas    = new System.Text.StringBuilder();
    var labelsJs = new System.Text.StringBuilder();
    var presJs   = new System.Text.StringBuilder();
    var ejecutJs = new System.Text.StringBuilder();
    using (var conn = Conexion.GetConnection())
    {
        conn.Open();
        using var cmd = new SqlCommand("sp_reporte_cumplimiento_presupuesto", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@p_id_presupuesto", idPresupuesto);
        cmd.Parameters.AddWithValue("@p_anio",           anio);
        cmd.Parameters.AddWithValue("@p_mes",            mes);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            string  nombre     = r.GetString(0);
            decimal pres       = r.GetDecimal(1);
            decimal ejec       = r.GetDecimal(2);
            decimal diferencia = pres - ejec;
            decimal porcentaje = pres > 0 ? (ejec / pres) * 100 : 0;
            string  color      = porcentaje < 80 ? "#2e7d32" : porcentaje <= 100 ? "#f57f17" : "#c62828";
            string  badge      = porcentaje < 80 ? "#e8f5e9" : porcentaje <= 100 ? "#fffde7" : "#ffebee";
            string  icono      = porcentaje < 80 ? "✓" : porcentaje <= 100 ? "⚠" : "✗";
            filas.Append($@"<tr>
                <td>{nombre}</td><td>L. {pres:N2}</td><td>L. {ejec:N2}</td><td>L. {diferencia:N2}</td>
                <td><span style='background:{badge};color:{color};padding:2px 8px;border-radius:99px;font-size:12px;font-weight:bold;'>{porcentaje:N1}% {icono}</span></td>
            </tr>");
            labelsJs.Append($"'{nombre}',");
            presJs.Append($"{pres.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)},");
            ejecutJs.Append($"{ejec.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)},");
        }
    }
    return $@"<!DOCTYPE html><html><head><style>{CssBase}</style></head><body>
        <h1>Reporte 3 — Cumplimiento de Presupuesto</h1>
        <p class='meta'>Período: {mes}/{anio}</p>
        <canvas id='chart3' height='120'></canvas>
        <table>
            <tr><th>Subcategoría</th><th>Presupuestado</th><th>Ejecutado</th><th>Diferencia</th><th>% Ejecución</th></tr>
            {filas}
        </table>
        <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
        <script>
        new Chart(document.getElementById('chart3'), {{
            type: 'bar',
            data: {{ labels: [{labelsJs}], datasets: [
                {{ label: 'Presupuestado', data: [{presJs}],  backgroundColor: '#90caf9' }},
                {{ label: 'Ejecutado',     data: [{ejecutJs}], backgroundColor: '#ef9a9a' }}
            ]}},
            options: {{ responsive: true, plugins: {{ legend: {{ position: 'bottom' }} }}, scales: {{ y: {{ beginAtZero: true }} }} }}
        }});
        </script>
        {Footer}
    </body></html>";
}

// ══════════════════════════════════════════════════════════
// REPORTE 4
// ══════════════════════════════════════════════════════════
private static async Task<string> ObtenerHtmlReporte4(int anioDesde, int mesDesde, int anioHasta, int mesHasta)
{
    var categorias = new List<(int id, string nombre)>();
    var meses      = new List<string>();
    var datos      = new Dictionary<string, List<decimal>>();
    using (var conn = Conexion.GetConnection())
    {
        conn.Open();
        using var cmd = new SqlCommand("sp_reporte_categorias_gasto", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            categorias.Add((r.GetInt32(0), r.GetString(1)));
            datos[r.GetString(1)] = new List<decimal>();
        }
    }
    int anio = anioDesde, mes = mesDesde;
    while (anio < anioHasta || (anio == anioHasta && mes <= mesHasta))
    {
        meses.Add($"{mes}/{anio}");
        foreach (var cat in categorias)
        {
            decimal total = EjecutarFuncionDecimal(
                "SELECT dbo.fn_obtener_total_ejecutado_categoria_mes(@c, @a, @m)",
                cmd => { cmd.Parameters.AddWithValue("@c", cat.id); cmd.Parameters.AddWithValue("@a", anio); cmd.Parameters.AddWithValue("@m", mes); });
            datos[cat.nombre].Add(total);
        }
        mes++;
        if (mes > 12) { mes = 1; anio++; }
    }
    string[] colores  = { "#f44336","#2196f3","#4caf50","#ff9800","#9c27b0","#00bcd4","#795548","#e91e63" };
    string   labelsJs = string.Join(",", meses.Select(m => $"'{m}'"));
    var datasetsJs    = new System.Text.StringBuilder();
    int i = 0;
    foreach (var kvp in datos)
    {
        string color   = colores[i % colores.Length];
        string valores = string.Join(",", kvp.Value.Select(v => v.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)));
        datasetsJs.Append($@"{{ label: '{kvp.Key}', data: [{valores}], borderColor: '{color}', backgroundColor: '{color}33', tension: 0.3, fill: false }},");
        i++;
    }
    return $@"<!DOCTYPE html><html><head><style>{CssBase}</style></head><body>
        <h1>Reporte 4 — Tendencia de Gastos por Categoría</h1>
        <p class='meta'>Período: {mesDesde}/{anioDesde} — {mesHasta}/{anioHasta}</p>
        <canvas id='chart4' height='120'></canvas>
        <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
        <script>
        new Chart(document.getElementById('chart4'), {{
            type: 'line',
            data: {{ labels: [{labelsJs}], datasets: [{datasetsJs}] }},
            options: {{ responsive: true, plugins: {{ legend: {{ position: 'bottom' }} }}, scales: {{ y: {{ beginAtZero: true }} }} }}
        }});
        </script>
        {Footer}
    </body></html>";
}

// ══════════════════════════════════════════════════════════
// REPORTE 5
// ══════════════════════════════════════════════════════════
private static async Task<string> ObtenerHtmlReporte5(int anio, int mes)
{
    var filas   = new System.Text.StringBuilder();
    int pagadas = 0, pendientes = 0, porVencer = 0, vencidas = 0;
    using (var conn = Conexion.GetConnection())
    {
        conn.Open();
        using var cmd = new SqlCommand("sp_reporte_estado_obligaciones", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@p_anio", anio);
        cmd.Parameters.AddWithValue("@p_mes",  mes);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            int     idObl  = r.GetInt32(0);
            string  nombre = r.GetString(1);
            decimal monto  = r.GetDecimal(2);
            int     dia    = r.GetByte(3);
            bool    pagada = !r.IsDBNull(4);
            int     dias   = EjecutarFuncionInt("SELECT dbo.fn_dias_hasta_vencimiento(@id)", c => c.Parameters.AddWithValue("@id", idObl));
            string estado, color, badge;
            if      (pagada)   { estado = "Pagada";                       color = "#2e7d32"; badge = "#e8f5e9"; pagadas++;    }
            else if (dias < 0) { estado = $"Vencida ({Math.Abs(dias)}d)"; color = "#c62828"; badge = "#ffebee"; vencidas++;   }
            else if (dias < 3) { estado = $"Por vencer ({dias}d)";        color = "#e65100"; badge = "#fff3e0"; porVencer++;  }
            else               { estado = $"Pendiente ({dias}d)";         color = "#f57f17"; badge = "#fffde7"; pendientes++; }
            string ultimoPago = pagada ? r.GetDateTime(4).ToString("dd/MM/yyyy") : "—";
            filas.Append($@"<tr>
                <td>{nombre}</td><td>L. {monto:N2}</td><td>Día {dia}</td><td>{ultimoPago}</td>
                <td><span style='background:{badge};color:{color};padding:2px 8px;border-radius:99px;font-size:12px;font-weight:bold;'>{estado}</span></td>
            </tr>");
        }
    }
    int    total   = pagadas + pendientes + porVencer + vencidas;
    string pctPag  = total > 0 ? (pagadas    * 100.0 / total).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) : "0";
    string pctPend = total > 0 ? (pendientes * 100.0 / total).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) : "0";
    string pctPorV = total > 0 ? (porVencer  * 100.0 / total).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) : "0";
    string pctVenc = total > 0 ? (vencidas   * 100.0 / total).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) : "0";
    return $@"<!DOCTYPE html><html><head><style>{CssBase}
        .layout {{ display: flex; gap: 30px; align-items: flex-start; }}
        .tabla  {{ flex: 2; }} .grafico {{ flex: 1; max-width: 300px; }}
    </style></head><body>
        <h1>Reporte 5 — Estado de Obligaciones Fijas</h1>
        <p class='meta'>Período: {mes}/{anio}</p>
        <div class='layout'>
            <div class='tabla'><table>
                <tr><th>Obligación</th><th>Monto</th><th>Vencimiento</th><th>Último pago</th><th>Estado</th></tr>
                {filas}
            </table></div>
            <div class='grafico'><canvas id='chart5'></canvas></div>
        </div>
        <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
        <script>
        new Chart(document.getElementById('chart5'), {{
            type: 'doughnut',
            data: {{ labels: ['Pagadas','Pendientes','Por vencer','Vencidas'],
                     datasets: [{{ data: [{pctPag},{pctPend},{pctPorV},{pctVenc}], backgroundColor: ['#4caf50','#ff9800','#e65100','#f44336'] }}] }},
            options: {{ plugins: {{ legend: {{ position: 'bottom' }} }} }}
        }});
        </script>
        {Footer}
    </body></html>";
}

// ══════════════════════════════════════════════════════════
// REPORTE 6
// ══════════════════════════════════════════════════════════
private static async Task<string> ObtenerHtmlReporte6(int idPresupuesto, int anio, int mes)
{
    var filas    = new System.Text.StringBuilder();
    var labelsJs = new System.Text.StringBuilder();
    var presJs   = new System.Text.StringBuilder();
    var ejecutJs = new System.Text.StringBuilder();
    using (var conn = Conexion.GetConnection())
    {
        conn.Open();
        using var cmd = new SqlCommand("sp_reporte_progreso_ahorros", conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@p_id_presupuesto", idPresupuesto);
        cmd.Parameters.AddWithValue("@p_anio",           anio);
        cmd.Parameters.AddWithValue("@p_mes",            mes);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            string  nombre = r.GetString(0);
            decimal pres = r.GetDecimal(1);
            decimal ejec = r.GetDecimal(2);
            decimal porcentaje = pres > 0 ? (ejec / pres) * 100 : 0;
            string  color = porcentaje >= 100 ? "#2e7d32" : porcentaje >= 50 ? "#f57f17" : "#c62828";
            filas.Append($@"<tr>
                <td>{nombre}</td><td>L. {pres:N2}</td><td>L. {ejec:N2}</td>
                <td style='color:{color};font-weight:bold;'>{porcentaje:N1}%</td>
            </tr>");
            labelsJs.Append($"'{nombre}',");
            presJs.Append($"{pres.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)},");
            ejecutJs.Append($"{ejec.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)},");
        }
    }
    return $@"<!DOCTYPE html><html><head><style>{CssBase}</style></head><body>
        <h1>Reporte 6 — Progreso de Ahorros</h1>
        <p class='meta'>Período: {mes}/{anio}</p>
        <canvas id='chart6' height='120'></canvas>
        <table>
            <tr><th>Subcategoría</th><th>Presupuestado</th><th>Ejecutado</th><th>% Cumplido</th></tr>
            {filas}
        </table>
        <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
        <script>
        new Chart(document.getElementById('chart6'), {{
            type: 'bar',
            data: {{ labels: [{labelsJs}], datasets: [
                {{ label: 'Presupuestado', data: [{presJs}],  backgroundColor: '#90caf9' }},
                {{ label: 'Ejecutado',     data: [{ejecutJs}], backgroundColor: '#4caf50' }}
            ]}},
            options: {{ responsive: true, plugins: {{ legend: {{ position: 'bottom' }} }}, scales: {{ y: {{ beginAtZero: true }} }} }}
        }});
        </script>
        {Footer}
    </body></html>";
}
        
    }
}