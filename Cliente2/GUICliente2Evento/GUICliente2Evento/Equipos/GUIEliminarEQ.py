import tkinter as tk
from tkinter import ttk, messagebox
import requests

API_BASE = "http://localhost:8091/equipos/"
API_EVENTOS = "http://localhost:8091/eventos/"

class GUIEliminarEQ(tk.Toplevel):
    def __init__(self, master=None):
        super().__init__(master)
        self.title("Eliminar Equipo")
        self.geometry("600x450")
        self.configure(bg="white")
        self.resizable(False, False)
        
        self.equipo_actual = None

       
        tk.Label(self, text="Eliminar Equipo",
                 font=("Verdana", 16, "bold"), bg="white").place(x=200, y=20)

       
        tk.Label(self, text="ID Equipo:", font=("Verdana", 11), bg="white").place(x=50, y=80)
        self.id_entry = tk.Entry(self, font=("Verdana", 11))
        self.id_entry.place(x=200, y=80, width=280)

        
        tk.Button(self, text="Buscar", font=("Verdana", 10, "bold"),
                  bg="lightblue", command=self.buscar_equipo).place(x=200, y=120, width=100)
        tk.Button(self, text="Cerrar", font=("Verdana", 10, "bold"),
                  bg="lightblue", command=self.destroy).place(x=330, y=120, width=100)

        
        labels = ["Nombre:", "Ciudad Origen:", "Numero Jugadores:", "Puntaje:", "Evento Deportivo:"]
        self.entries = {}
        y_inicial = 180
        
        for i, text in enumerate(labels):
            tk.Label(self, text=text, font=("Verdana", 11), bg="white").place(x=50, y=y_inicial + i*50)
            
            if text == "Evento Deportivo:":
                
                entry = tk.Entry(self, font=("Verdana", 11), state="readonly")
            else:
                entry = tk.Entry(self, font=("Verdana", 11), state="readonly")
            
            entry.place(x=200, y=y_inicial + i*50, width=280)
            self.entries[text] = entry
  
        
        tk.Button(self, text="Eliminar", font=("Verdana", 10, "bold"),
                  bg="lightblue", command=self.eliminar_equipo).place(x=250, y=400, width=100)

   
    def buscar_equipo(self):
        idEquipo = self.id_entry.get().strip()

        if not idEquipo:
            messagebox.showwarning("Campo vacío", "Por favor ingresa un ID válido.")
            return

        try:
            idEquipo = int(idEquipo)
            auth = ("admin", "admin")
            headers = {"Accept": "application/json"}
            url = f"{API_BASE}{idEquipo}"
            resp = requests.get(url, auth=auth, headers=headers)

            if resp.status_code == 200:
                data = resp.json()
                self.equipo_actual = data
                self.llenar_campos(data)
            elif resp.status_code == 401:
                messagebox.showerror("Acceso denegado", "Credenciales inválidas (admin/admin).")
            elif resp.status_code == 404:
                messagebox.showinfo("Sin resultados", f"No existe equipo con ID: {idEquipo}")
                self.limpiar_campos()
            else:
                messagebox.showerror("Error", f"Error al buscar equipo.\nCódigo: {resp.status_code}")
                self.limpiar_campos()

        except ValueError:
            messagebox.showwarning("Validación", "El ID del equipo debe ser numérico.")
        except requests.exceptions.ConnectionError:
            messagebox.showerror("Error de conexión", 
                "No se pudo conectar al servidor.\n¿Está corriendo el backend en http://localhost:8091?")
        except Exception as e:
            messagebox.showerror("Error", f"No se pudo conectar al servidor.\n{e}")

 
    def llenar_campos(self, data):
        
        for entry in self.entries.values():
            entry.config(state="normal")
            entry.delete(0, tk.END)
        
        self.entries["Nombre:"].insert(0, data.get("nombre", ""))
        self.entries["Ciudad Origen:"].insert(0, data.get("ciudadOrigen", ""))
        self.entries["Numero Jugadores:"].insert(0, data.get("numeroJugadores", ""))
        self.entries["Puntaje:"].insert(0, data.get("puntaje", ""))
        
        
        evento = data.get("eventoDeportivo")
        if evento and evento.get("idEvento"):
            self.entries["Evento Deportivo:"].insert(0, evento.get("idEvento"))
        else:
            self.entries["Evento Deportivo:"].insert(0, "Sin evento")
        
        # Volver a readonly
        for entry in self.entries.values():
            entry.config(state="readonly")

    
    def limpiar_campos(self):
        for entry in self.entries.values():
            entry.config(state="normal")
            entry.delete(0, tk.END)
            entry.config(state="readonly")
        self.id_entry.delete(0, tk.END)
        self.equipo_actual = None

    
    def eliminar_equipo(self):
        if not self.equipo_actual:
            messagebox.showwarning("Advertencia", "Primero debes buscar un equipo para eliminar.")
            return

        idEquipo = self.equipo_actual.get("idEquipo")
        nombre = self.equipo_actual.get("nombre", "")

       
        respuesta = messagebox.askyesno(
            "Confirmar eliminación",
            f"¿Estás seguro de que deseas eliminar el equipo:\n\n"
            f"ID: {idEquipo}\n"
            f"Nombre: {nombre}\n\n"
            f"Esta acción no se puede deshacer."
        )

        if not respuesta:
            return

        try:
            auth = ("admin", "admin")
            url = f"{API_BASE}{idEquipo}"
            resp = requests.delete(url, auth=auth)

            if resp.status_code in (200, 204):
                messagebox.showinfo("Éxito", f"Equipo '{nombre}' eliminado correctamente.")
                self.limpiar_campos()
            elif resp.status_code == 404:
                messagebox.showerror("Error", f"No se encontró el equipo con ID: {idEquipo}")
            elif resp.status_code == 401:
                messagebox.showerror("Acceso denegado", "Credenciales inválidas.")
            else:
                messagebox.showerror("Error", f"No se pudo eliminar el equipo.\nCódigo: {resp.status_code}\n{resp.text}")

        except requests.exceptions.ConnectionError:
            messagebox.showerror("Error de conexión", 
                "No se pudo conectar al servidor.\n¿Está corriendo el backend?")
        except Exception as e:
            messagebox.showerror("Error", f"Error inesperado:\n{str(e)}")


if __name__ == "__main__":
    root = tk.Tk()
    root.withdraw()
    GUIEliminarEQ(root)
    root.mainloop()