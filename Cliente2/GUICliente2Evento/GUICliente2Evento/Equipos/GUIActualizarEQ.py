import tkinter as tk
from tkinter import ttk, messagebox
import requests

API_BASE = "http://localhost:8091/equipos/"
API_EVENTOS = "http://localhost:8091/eventos/"

class GUIActualizarEQ(tk.Toplevel):
    def __init__(self, master=None):
        super().__init__(master)
        self.title("Actualizar Equipo")
        self.geometry("600x550")
        self.configure(bg="white")
        self.resizable(False, False)
        
        self.eventos_disponibles = []
        self.evento_actual_id = None

        
        tk.Label(self, text="Actualizar Equipo",
                 font=("Verdana", 16, "bold"), bg="white").place(x=200, y=20)

        
        tk.Label(self, text="ID Equipo:", font=("Verdana", 11), bg="white").place(x=50, y=80)
        self.id_entry = tk.Entry(self, font=("Verdana", 11))
        self.id_entry.place(x=200, y=80, width=280)

       
        tk.Button(self, text="Buscar", font=("Verdana", 10, "bold"),
                  bg="lightblue", command=self.buscar_equipo).place(x=200, y=120, width=100)
        tk.Button(self, text="Cerrar", font=("Verdana", 10, "bold"),
                  bg="lightblue", command=self.destroy).place(x=330, y=120, width=100)

        
        labels = ["Nombre:", "Ciudad Origen:", "Numero Jugadores:", "Puntaje:"]
        self.entries = {}
        y_inicial = 180
        for i, text in enumerate(labels):
            tk.Label(self, text=text, font=("Verdana", 11), bg="white").place(x=50, y=y_inicial + i*50)
            entry = tk.Entry(self, font=("Verdana", 11))
            entry.place(x=200, y=y_inicial + i*50, width=280)
            self.entries[text] = entry

        
        tk.Label(self, text="Evento Deportivo:", font=("Verdana", 11), bg="white").place(x=50, y=380)
        self.evento_combo = ttk.Combobox(self, font=("Verdana", 11), state="readonly")
        self.evento_combo.place(x=200, y=380, width=280)
        
       
        tk.Button(self, text="Editar", font=("Verdana", 10, "bold"),
                  bg="lightblue", command=lambda: self.habilitar_campos(True)).place(x=180, y=500, width=100)
        tk.Button(self, text="Guardar", font=("Verdana", 10, "bold"),
                  bg="lightblue", command=self.actualizar_equipo).place(x=320, y=500, width=100)

        self.habilitar_campos(False)
        self.cargar_eventos()

    
    def cargar_eventos(self):
        try:
            auth = ("admin", "admin")
            headers = {"Accept": "application/json"}
            
            print(f"Intentando conectar a: {API_EVENTOS}")  
            resp = requests.get(API_EVENTOS, auth=auth, headers=headers, timeout=5)
            
            print(f"Status code: {resp.status_code}")  
            
            if resp.status_code == 200:
                eventos = resp.json()
                print(f"Eventos recibidos: {len(eventos)}")  
                
                self.eventos_disponibles = eventos
                
                
                opciones = ["Sin evento"] + [e.get("nombre", f"Evento {e.get('idEvento')}") for e in eventos]
                self.evento_combo['values'] = opciones
                self.evento_combo.set("Sin evento")
                
                print(f"Eventos cargados exitosamente: {len(eventos)}")  
            elif resp.status_code == 401:
                messagebox.showerror("Error de autenticación")
                self.evento_combo['values'] = ["Sin evento"]
                self.evento_combo.set("Sin evento")
            elif resp.status_code == 404:
                messagebox.showwarning("Advertencia eventos no se encontraron")
                self.evento_combo['values'] = ["Sin evento"]
                self.evento_combo.set("Sin evento")
            else:
                messagebox.showwarning("Advertencia", 
                    f"No se pudieron cargar los eventos deportivos.\nCódigo HTTP: {resp.status_code}")
                self.evento_combo['values'] = ["Sin evento"]
                self.evento_combo.set("Sin evento")
        
        except requests.exceptions.ConnectionError:
            messagebox.showerror("Error de conexión")
            self.evento_combo['values'] = ["Sin evento"]
            self.evento_combo.set("Sin evento")
        
        except requests.exceptions.Timeout:
            messagebox.showerror("Timeout", 
                "El servidor tardó demasiado en responder.")
            self.evento_combo['values'] = ["Sin evento"]
            self.evento_combo.set("Sin evento")
        
        except Exception as e:
            messagebox.showerror("Error inesperado", 
                f"Error al cargar eventos:\n{type(e).__name__}: {str(e)}")
            self.evento_combo['values'] = ["Sin evento"]
            self.evento_combo.set("Sin evento")
            print(f"Error completo: {e}")  

    
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
                self.llenar_campos(data)
                self.habilitar_campos(False)
            elif resp.status_code == 401:
                messagebox.showerror("Acceso denegado", "Credenciales inválidas (admin/admin).")
            else:
                messagebox.showinfo("Sin resultados", f"No existe equipo con ID: {idEquipo}")
                self.limpiar_campos()

        except ValueError:
            messagebox.showwarning("Validación", "El ID del equipo debe ser numérico.")
        except Exception as e:
            messagebox.showerror("Error", f"No se pudo conectar al servidor.\n{e}")

    
    def llenar_campos(self, data):
        for label, entry in self.entries.items():
            entry.config(state="normal")
            entry.delete(0, tk.END)
        
        self.entries["Nombre:"].insert(0, data.get("nombre", ""))
        self.entries["Ciudad Origen:"].insert(0, data.get("ciudadOrigen", ""))
        self.entries["Numero Jugadores:"].insert(0, data.get("numeroJugadores", ""))
        self.entries["Puntaje:"].insert(0, data.get("puntaje", ""))
        
      
        nombre_evento = data.get("nombreEvento")
        if nombre_evento:
            self.evento_combo.set(nombre_evento)
        else:
            self.evento_combo.set("Sin evento")

    
    def habilitar_campos(self, habilitar):
        for entry in self.entries.values():
            entry.config(state="normal" if habilitar else "readonly")
        self.evento_combo.config(state="readonly" if habilitar else "disabled")

   
    def limpiar_campos(self):
        for entry in self.entries.values():
            entry.config(state="normal")
            entry.delete(0, tk.END)
            entry.config(state="readonly")
        self.id_entry.delete(0, tk.END)
        self.evento_combo.set("Sin evento")
        self.evento_actual_id = None

    

    def actualizar_equipo(self):
        idEquipo = self.id_entry.get().strip()

        try:
            idEquipo = int(idEquipo)
            
            
            evento_seleccionado = self.evento_combo.get()
            id_evento = None
            
            if evento_seleccionado != "Sin evento":
                for evento in self.eventos_disponibles:
                    if evento.get("nombre") == evento_seleccionado:
                        id_evento = evento.get("idEvento")
                        break

            
            payload = {
                "idEquipo": idEquipo,
                "nombre": self.entries["Nombre:"].get(),
                "ciudadOrigen": self.entries["Ciudad Origen:"].get(),
                "numeroJugadores": int(self.entries["Numero Jugadores:"].get()),
                "puntaje": float(self.entries["Puntaje:"].get()),
                "eventoDeportivo": {"idEvento": id_evento} if id_evento else None
            }

            import json
            print("\n=== JSON ENVIADO AL BACKEND ===")
            print(json.dumps(payload, indent=4))
            print("================================")

            
            auth = ("admin", "admin")
            resp = requests.put(f"{API_BASE}{idEquipo}", json=payload, auth=auth)

            print(f"Status code: {resp.status_code}")
            print(f"Respuesta del servidor: {resp.text}")

            
            if resp.status_code in (200, 204):
                messagebox.showinfo("Éxito", "Equipo actualizado correctamente.")
                self.limpiar_campos()
            elif resp.status_code == 400:
                messagebox.showerror("Error", f"Solicitud inválida.\n{resp.text}")
            elif resp.status_code == 415:
                messagebox.showerror("Error", "El servidor no aceptó el formato del contenido (415).")
            elif resp.status_code == 404:
                messagebox.showerror("Error", f"No existe equipo con ID {idEquipo}.")
            else:
                messagebox.showerror("Error", f"Error inesperado.\nStatus: {resp.status_code}\n{resp.text}")

        except ValueError:
            messagebox.showwarning("Validación", "Número de jugadores y puntaje deben ser numéricos.")
        except Exception as e:
            messagebox.showerror("Error", f"Error al actualizar: {str(e)}")




if __name__ == "__main__":
    root = tk.Tk()
    root.withdraw()
    GUIActualizarEQ(root)
    root.mainloop()