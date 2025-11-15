import tkinter as tk
from tkinter import ttk, messagebox
import requests

API_SEDES = "http://localhost:8092/sedes/"
API_EVENTOS = "http://localhost:8091/eventos/"

class GUIActualizarSedeDeportiva(tk.Toplevel):
    def __init__(self, master=None):
        super().__init__(master)
        self.title("Actualizar Sede Deportiva")
        self.geometry("600x650")
        self.configure(bg="white")
        self.resizable(False, False)

        self.eventos_disponibles = []

        tk.Label(self, text="Actualizar Sede Deportiva",
                 font=("Verdana", 16, "bold"), bg="white").place(x=150, y=20)

        # ===== ID SEDE =====
        tk.Label(self, text="ID Sede:", font=("Verdana", 11), bg="white").place(x=50, y=80)
        self.id_entry = tk.Entry(self, font=("Verdana", 11))
        self.id_entry.place(x=200, y=80, width=280)

        tk.Button(self, text="Buscar", font=("Verdana", 10, "bold"),
                  bg="lightblue", command=self.buscar_sede).place(x=200, y=120, width=100)

        tk.Button(self, text="Cerrar", font=("Verdana", 10, "bold"),
                  bg="lightblue", command=self.destroy).place(x=330, y=120, width=100)

        # ===== FORMULARIO =====
        labels = [
            "Nombre:", "Capacidad:", "Dirección:",
            "Costo Mantenimiento:", "Fecha Creación:"
        ]

        self.entries = {}
        y_inicial = 180

        for i, text in enumerate(labels):
            tk.Label(self, text=text, font=("Verdana", 11), bg="white").place(
                x=50, y=y_inicial + i * 50
            )
            entry = tk.Entry(self, font=("Verdana", 11))
            entry.place(x=250, y=y_inicial + i * 50, width=280)
            self.entries[text] = entry

        # ===== Cubierta =====
        tk.Label(self, text="Cubierta:", font=("Verdana", 11), bg="white").place(
            x=50, y=y_inicial + 5 * 50
        )
        self.combo_cubierta = ttk.Combobox(
            self, font=("Verdana", 11), state="readonly", values=["True", "False"]
        )
        self.combo_cubierta.place(x=250, y=y_inicial + 5 * 50, width=280)

        # ===== Evento Deportivo =====
        tk.Label(self, text="Evento Deportivo:", font=("Verdana", 11), bg="white").place(
            x=50, y=y_inicial + 6 * 50
        )
        self.evento_combo = ttk.Combobox(self, font=("Verdana", 11), state="readonly")
        self.evento_combo.place(x=250, y=y_inicial + 6 * 50, width=280)

        # ===== Botones =====
        tk.Button(self, text="Editar", font=("Verdana", 10, "bold"),
                  bg="lightblue", command=lambda: self.habilitar_campos(True)).place(
            x=180, y=600, width=100
        )

        tk.Button(self, text="Guardar", font=("Verdana", 10, "bold"),
                  bg="lightblue", command=self.actualizar_sede).place(
            x=320, y=600, width=100
        )

        # Iniciar
        self.habilitar_campos(False)
        self.cargar_eventos()


    def cargar_eventos(self):
        try:
            resp = requests.get(API_EVENTOS, auth=("admin", "admin"))

            if resp.status_code == 200:
                eventos = resp.json()
                self.eventos_disponibles = eventos

                opciones = ["Sin evento"] + [e["nombre"] for e in eventos]
                self.evento_combo["values"] = opciones
                self.evento_combo.set("Sin evento")
            else:
                self.evento_combo["values"] = ["Sin evento"]
                self.evento_combo.set("Sin evento")

        except Exception as e:
            print("Error cargar_eventos:", e)
            self.evento_combo["values"] = ["Sin evento"]
            self.evento_combo.set("Sin evento")


    def buscar_sede(self):
        idSede = self.id_entry.get().strip()

        if not idSede:
            messagebox.showwarning("Validación", "Ingrese un ID válido.")
            return

        try:
            idSede = int(idSede)
            url = API_SEDES + str(idSede)
            print(f"GET {url}")
            resp = requests.get(url, auth=("admin", "admin"))

            print("Status:", resp.status_code)
            if resp.status_code == 200:
                data = resp.json()
                # DEBUG: mostrar el JSON recibido
                print("\n=== JSON RECIBIDO ===")
                print(data)
                print("=====================\n")

                self.llenar_campos(data)
                # dejar campos en readonly por defecto
                self.habilitar_campos(False)
            else:
                messagebox.showinfo("Sin resultados", f"No existe Sede con ID {idSede}")
                self.limpiar_campos()

        except Exception as e:
            messagebox.showerror("Error", f"No se pudo conectar al servidor.\n{e}")


    def llenar_campos(self, data):
        # Primero, poner TODOS los Entry en normal para permitir escritura
        for entry in self.entries.values():
            try:
                entry.config(state="normal")
            except Exception:
                pass

        # Mapeo campo label -> clave JSON (ajustable si tu API usa otros nombres)
        field_map = {
            "Nombre:": "nombre",
            "Capacidad:": "capacidad",
            "Dirección:": "direccion",
            "Costo Mantenimiento:": "costoMantenimiento",
            "Fecha Creación:": "fechaCreacion"
        }

        # Imprimir keys disponibles en el JSON para diagnóstico
        try:
            print("Claves en JSON:", list(data.keys()))
        except Exception:
            print("No se pudieron listar claves del JSON")

        # Intentar llenar cada campo y mostrar por consola lo que se escribe
        for label, json_key in field_map.items():
            entry = self.entries.get(label)
            if entry is None:
                print(f"[WARN] No existe entry para label '{label}'")
                continue

            # valor que viene del JSON (si no existe, se muestra '')
            valor = data.get(json_key, "")
            # Mostrar intento en consola
            print(f"Filling '{label}' from JSON key '{json_key}' -> value: {repr(valor)}")
            try:
                entry.delete(0, tk.END)
                entry.insert(0, str(valor))
            except Exception as e:
                print(f"[ERROR] al insertar en {label}: {e}")

        # Cubierta (combobox)
        try:
            cub = data.get("cubierta")
            print("Valor 'cubierta' en JSON:", cub)
            self.combo_cubierta.set("True" if cub else "False")
        except Exception as e:
            print("Error al setear cubierta:", e)

        # Evento asociado: API devuelve idEventoAsociado (ID) — traducir a nombre
        try:
            idEvento = data.get("idEventoAsociado")
            print("Valor 'idEventoAsociado' en JSON:", idEvento)
            if idEvento is None:
                self.evento_combo.set("Sin evento")
            else:
                nombreEvento = next((e["nombre"] for e in self.eventos_disponibles if e.get("idEvento") == idEvento), None)
                print("Nombre resuelto del evento:", nombreEvento)
                self.evento_combo.set(nombreEvento if nombreEvento else "Sin evento")
        except Exception as e:
            print("Error al setear evento:", e)
            self.evento_combo.set("Sin evento")

        # Por seguridad, dejar los entries en readonly (modo no edición)
        for entry in self.entries.values():
            try:
                entry.config(state="readonly")
            except Exception:
                pass


    def habilitar_campos(self, habilitar):
        for entry in self.entries.values():
            entry.config(state="normal" if habilitar else "readonly")

        if habilitar:
            self.combo_cubierta.config(state="readonly")
            self.evento_combo.config(state="readonly")
        else:
            self.combo_cubierta.config(state="disabled")
            self.evento_combo.config(state="disabled")


    def limpiar_campos(self):
        for entry in self.entries.values():
            entry.config(state="normal")
            entry.delete(0, tk.END)
            entry.config(state="readonly")

        self.combo_cubierta.set("False")
        self.evento_combo.set("Sin evento")
        self.id_entry.delete(0, tk.END)

    def actualizar_sede(self):
        try:
            idSede = int(self.id_entry.get())

            # Buscar idEvento según nombre seleccionado
            evento_selected = self.evento_combo.get()
            id_evento = None

            if evento_selected != "Sin evento":
                for e in self.eventos_disponibles:
                    if e["nombre"] == evento_selected:
                        id_evento = e["idEvento"]

            capacidad = self.entries["Capacidad:"].get()
            costo = self.entries["Costo Mantenimiento:"].get()

            if capacidad == "" or costo == "":
                messagebox.showwarning("Validación", "Capacidad y costo no pueden estar vacíos.")
                return

            payload = {
                "nombre": self.entries["Nombre:"].get(),
                "capacidad": int(capacidad),
                "direccion": self.entries["Dirección:"].get(),
                "costoMantenimiento": float(costo),
                "fechaCreacion": self.entries["Fecha Creación:"].get(),
                "cubierta": self.combo_cubierta.get() == "True",
                "idEventoAsociado": id_evento
            }

            resp = requests.put(API_SEDES + str(idSede), json=payload, auth=("admin", "admin"))

            if resp.status_code in (200, 204):
                messagebox.showinfo("Éxito", "Sede actualizada correctamente.")
                self.limpiar_campos()
            else:
                messagebox.showerror("Error", f"No se pudo actualizar.\n{resp.text}")

        except ValueError:
            messagebox.showwarning("Validación", "Capacidad y costo deben ser numéricos.")
        except Exception as e:
            messagebox.showerror("Error", f"Error al actualizar:\n{e}")


# Ejecutar Independiente
if __name__ == "__main__":
    root = tk.Tk()
    root.withdraw()
    GUIActualizarSedeDeportiva(root)
    root.mainloop()
