import tkinter as tk
from tkinter import ttk, messagebox
import requests

API_BASE = "http://localhost:8091/equipos/"  

class GUIListarEQ(tk.Toplevel):
    def __init__(self, master=None):
        super().__init__(master)
        self.title("Listar Equipos")
        self.geometry("850x500")  
        self.configure(bg="white")
        self.resizable(False, False)
        
        self.crear_interfaz()
    
    def crear_interfaz(self):
        # Título
        tk.Label(self, text="🏆 Listar Equipos", 
                 font=("Verdana", 16, "bold"), bg="white", fg="black").place(x=320, y=20)
        
        
        columnas = ("idEquipo", "Nombre", "Ciudad Origen", "Numero Jugadores", "Puntaje", "Evento Deportivo")
        self.tabla = ttk.Treeview(self, columns=columnas, show="headings", height=15)
        self.tabla.place(x=20, y=80, width=810, height=340)
        
        
        anchos = {
            "idEquipo": 80,
            "Nombre": 130,
            "Ciudad Origen": 130,
            "Numero Jugadores": 120,
            "Puntaje": 80,
            "Evento Deportivo": 150
        }
        
        for col in columnas:
            self.tabla.heading(col, text=col)
            self.tabla.column(col, width=anchos.get(col, 130), anchor="center")
        
        
        scrollbar_y = ttk.Scrollbar(self, orient="vertical", command=self.tabla.yview)
        self.tabla.configure(yscrollcommand=scrollbar_y.set)
        scrollbar_y.place(x=830, y=80, height=340)
        
       
        tk.Button(self, text="Listar", font=("Verdana", 11, "bold"), 
                  command=self.listar_equipos).place(x=270, y=440, width=120)
        
        tk.Button(self, text="Cerrar", font=("Verdana", 11, "bold"), 
                  command=self.destroy).place(x=460, y=440, width=120)
    
    def listar_equipos(self):
        try:
            auth = ("admin", "admin")  # Autenticación básica
            headers = {"Accept": "application/json"}
            resp = requests.get(API_BASE, auth=auth, headers=headers)
            
            
            if resp.status_code == 401:
                messagebox.showerror("Acceso denegado", "No autorizado. Verifica credenciales")
                return
            
            if resp.status_code == 200:
                equipos = resp.json()
                
                
                for item in self.tabla.get_children():
                    self.tabla.delete(item)
                
                
                for e in equipos:
                    
                    nombre_evento = e.get("nombreEvento") or "Sin evento"
                    
                    self.tabla.insert("", "end", values=(
                        e.get("idEquipo", ""),
                        e.get("nombre", ""),
                        e.get("ciudadOrigen", ""),
                        e.get("numeroJugadores", ""),
                        e.get("puntaje", ""),
                        nombre_evento  
                    ))
            else:
                messagebox.showerror("Error",
                                     f"No se pudieron obtener los equipos. Código HTTP: {resp.status_code}")
        
        except Exception as e:
            messagebox.showerror("Error de conexión", f"No se pudo conectar al backend.\n{e}")


if __name__ == "__main__":
    root = tk.Tk()
    root.withdraw()
    GUIListarEQ(root)
    root.mainloop()