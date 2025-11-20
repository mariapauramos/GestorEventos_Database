import tkinter as tk
from tkinter import messagebox
import requests

API_SEDES = "http://localhost:8092/sedes"
API_EVENTOS = "http://localhost:8091/eventos"

class GUIBuscarSedeDeportiva(tk.Toplevel):
    def __init__(self, master=None):
        super().__init__(master)
        self.title("Buscar Sede Deportiva")
        self.geometry("580x600")
        self.configure(bg="white")
        self.resizable(False, False)
        try:
            self.img = tk.PhotoImage(file="images/cultural.png")
            tk.Label(self, image=self.img, bg="white").place(x=50, y=10, width=80, height=80)
        except:
            pass
        tk.Label(self, text="BUSCAR SEDE DEPORTIVA",
                 font=("Verdana", 18, "bold"), bg="white").place(x=120, y=35)
        self.crear_formulario()
    
    def crear_formulario(self):
        tk.Label(self, text="ID Sede:", font=("Verdana", 11), bg="white").place(x=50, y=100)
        self.id_entry = tk.Entry(self, font=("Verdana", 11))
        self.id_entry.place(x=180, y=100, width=220)
        self.id_entry.focus_set()
        tk.Button(self, text="Buscar", font=("Verdana", 10, "bold"),
                  command=self.buscar_sede).place(x=420, y=100, width=90)
        labels = [
            "Nombre:", "Capacidad:", "Dirección:", "Costo Mant.:",
            "Fecha Creación:", "Cubierta:", "Evento Asociado:"
        ]
        self.entries = {}
        y_inicial = 160
        for i, text in enumerate(labels):
            tk.Label(self, text=text, font=("Verdana", 11), bg="white").place(
                x=50, y=y_inicial + i * 40
            )
            entry = tk.Entry(self, font=("Verdana", 11))
            entry.place(x=200, y=y_inicial + i * 40, width=320)
            entry.config(state="readonly")
            self.entries[text] = entry
        tk.Button(self, text="Cerrar", font=("Verdana", 11, "bold"),
                  command=self.destroy).place(x=250, y=530, width=100)
    
    def obtener_nombre_evento(self, id_evento):
        """Obtiene el nombre del evento dado su ID"""
        try:
            auth = ("admin", "admin")
            url = f"{API_EVENTOS}/{id_evento}"
            resp = requests.get(url, auth=auth, headers={"Accept": "application/json"})
            if resp.status_code == 200:
                evento = resp.json()
                return evento.get("nombre", f"Evento ID: {id_evento}")
            else:
                return f"Evento ID: {id_evento}"
        except Exception:
            return f"Evento ID: {id_evento}"
    
    def buscar_sede(self):
        idSede = self.id_entry.get().strip()
        if not idSede:
            messagebox.showwarning("Campo vacío", "Por favor ingresa un ID válido.")
            return
        try:
            auth = ("admin", "admin")
            url = f"{API_SEDES}/{idSede}"
            resp = requests.get(url, auth=auth, headers={"Accept": "application/json"})
            if resp.status_code == 200:
                data = resp.json()
                self.llenar_campos(data)
            else:
                messagebox.showinfo("Sin resultados",
                                    f"No existe una sede con ID: {idSede}")
                self.limpiar_campos()
                self.id_entry.focus_set()
        except Exception as e:
            messagebox.showerror("Error de conexión",
                                 f"No se pudo conectar al backend.\n{e}")
    
    def llenar_campos(self, data):
        self.limpiar_campos()
        for entry in self.entries.values():
            entry.config(state="normal")
        
        self.entries["Nombre:"].insert(0, data.get("nombre", ""))
        self.entries["Capacidad:"].insert(0, data.get("capacidad", ""))
        self.entries["Dirección:"].insert(0, data.get("direccion", ""))
        self.entries["Costo Mant.:"].insert(0, data.get("costoMantenimiento", ""))
        self.entries["Fecha Creación:"].insert(0, data.get("fechaCreacion", ""))
        self.entries["Cubierta:"].insert(0, "Sí" if data.get("cubierta") else "No")
        
        # Obtener el nombre del evento usando el ID
        id_evento = data.get("idEventoAsociado")
        if id_evento:
            nombre_evento = self.obtener_nombre_evento(id_evento)
            self.entries["Evento Asociado:"].insert(0, nombre_evento)
        else:
            self.entries["Evento Asociado:"].insert(0, "Sin evento")
        
        for entry in self.entries.values():
            entry.config(state="readonly")
    
    def limpiar_campos(self):
        for entry in self.entries.values():
            entry.config(state="normal")
            entry.delete(0, tk.END)
            entry.config(state="readonly")

if __name__ == "__main__":
    root = tk.Tk()
    root.withdraw()
    GUIBuscarSedeDeportiva(root)
    root.mainloop()