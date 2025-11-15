import tkinter as tk
from tkinter import ttk, messagebox
import requests

API_BASE = "http://localhost:8092/sedes/"
API_EVENTOS = "http://localhost:8092/eventos/"

class GUIListarSedeDeportiva(tk.Toplevel):
    def __init__(self, master=None):
        super().__init__(master)
        self.title("Listar Sedes Deportivas")
        self.geometry("1100x520")
        self.configure(bg="white")
        self.resizable(False, False)

        self.crear_interfaz()

    def crear_interfaz(self):

        tk.Label(self, text="📍 Listar Sedes Deportivas",
                 font=("Verdana", 16, "bold"), bg="white").place(x=340, y=20)

        columnas = (
            "idSede",
            "Nombre",
            "Capacidad",
            "Direccion",
            "Costo Mantenimiento",
            "Fecha Creacion",
            "Cubierta",
            "ID Evento",
            "Nombre Evento"
        )

        self.tabla = ttk.Treeview(self, columns=columnas, show="headings", height=15)
        self.tabla.place(x=20, y=80, width=1050, height=360)

        anchos = {
            "idSede": 70,
            "Nombre": 150,
            "Capacidad": 80,
            "Direccion": 150,
            "Costo Mantenimiento": 120,
            "Fecha Creacion": 110,
            "Cubierta": 80,
            "ID Evento": 80,
            "Nombre Evento": 150
        }

        for col in columnas:
            self.tabla.heading(col, text=col)
            self.tabla.column(col, width=anchos.get(col, 130), anchor="center")

        scrollbar_y = ttk.Scrollbar(self, orient="vertical", command=self.tabla.yview)
        self.tabla.configure(yscrollcommand=scrollbar_y.set)
        scrollbar_y.place(x=1070, y=80, height=360)

        tk.Button(self, text="Listar", font=("Verdana", 11, "bold"),
                  command=self.listar_sedes).place(x=400, y=460, width=120)

        tk.Button(self, text="Cerrar", font=("Verdana", 11, "bold"),
                  command=self.destroy).place(x=550, y=460, width=120)

    
    def obtener_nombre_evento(self, id_evento):
        try:
            if not id_evento:
                return "Sin evento"

            resp = requests.get(f"http://localhost:8091/eventos/{id_evento}",
                                auth=("admin", "admin"))

            if resp.status_code == 200:
                evento = resp.json()
                return evento.get("nombre", "Sin nombre")

            return "Evento no encontrado"

        except Exception as e:
            print("Error buscando evento:", e)
            return "Error"


    
    # Listar las sedes que existen
   
    def listar_sedes(self):
        try:
            auth = ("admin", "admin")
            headers = {"Accept": "application/json"}

            resp = requests.get(API_BASE, auth=auth, headers=headers)

            if resp.status_code == 401:
                messagebox.showerror("Acceso denegado", "No autorizado. Verifica credenciales")
                return

            if resp.status_code == 200:
                sedes = resp.json()

                for item in self.tabla.get_children():
                    self.tabla.delete(item)

                for s in sedes:
                    id_evento = s.get("idEventoAsociado", "")
                    nombre_evento = self.obtener_nombre_evento(id_evento)

                    self.tabla.insert("", "end", values=(
                        s.get("idSede", ""),
                        s.get("nombre", ""),
                        s.get("capacidad", ""),
                        s.get("direccion", ""),
                        s.get("costoMantenimiento", ""),
                        s.get("fechaCreacion", ""),
                        "Sí" if s.get("cubierta") else "No",
                        id_evento,
                        nombre_evento
                    ))

            else:
                messagebox.showerror("Error",
                                     f"No se pudieron obtener las sedes. Código HTTP: {resp.status_code}")

        except Exception as e:
            messagebox.showerror("Error de conexión",
                                 f"No se pudo conectar al backend.\n{e}")


if __name__ == "__main__":
    root = tk.Tk()
    root.withdraw()
    GUIListarSedeDeportiva(root)
    root.mainloop()
