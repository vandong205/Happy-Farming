import json
import struct
import numpy as np
import matplotlib.pyplot as plt
import tkinter as tk
from tkinter import filedialog, messagebox

# ================= DATA =================
object_map = None
walkable_map = None
object_names = {}

# ================= LOADERS =================
def load_object_json():
    global object_names
    path = filedialog.askopenfilename(filetypes=[("JSON files", "*.json")])
    if not path:
        return
    with open(path, "r", encoding="utf-8-sig") as f:
        object_names = json.load(f)
    lbl_json.config(text=path)

def load_object_bin():
    global object_map
    path = filedialog.askopenfilename(filetypes=[("BIN files", "*.bin")])
    if not path:
        return

    with open(path, "rb") as f:
        w, h = struct.unpack("2i", f.read(8))
        data = struct.unpack(f"{w*h}i", f.read(4 * w * h))

    object_map = np.array(data).reshape((h, w))
    lbl_objbin.config(text=path)

def load_walkable_bin():
    global walkable_map
    path = filedialog.askopenfilename(filetypes=[("BIN files", "*.bin")])
    if not path:
        return

    with open(path, "rb") as f:
        w, h, ox, oy = struct.unpack("4i", f.read(16))
        data = struct.unpack(f"{w*h}B", f.read(w * h))

    walkable_map = np.array(data).reshape((h, w))
    lbl_walkbin.config(text=path)

# ================= VISUAL =================
def visualize():
    if object_map is None:
        messagebox.showerror("Error", "Chưa load Object BIN")
        return

    plt.figure(figsize=(8, 8))

    ids = np.unique(object_map)
    base = plt.colormaps["tab20"]
    cmap = base.resampled(len(ids))
    img = plt.imshow(object_map, cmap=cmap, origin="lower")

    if show_walkable.get() and walkable_map is not None:
        if walkable_map.shape != object_map.shape:
            messagebox.showerror("Error", "Walkable map khác kích thước")
            return
        overlay = np.zeros((*walkable_map.shape, 4))
        overlay[walkable_map == 0] = [1, 0, 0, 0.4]
        plt.imshow(overlay, origin="lower")

    cbar = plt.colorbar(img, ticks=ids)
    cbar.ax.set_yticklabels(
        [object_names.get(str(i), f"ID {i}") for i in ids]
    )

    plt.title("Unity Map Viewer (Object + Walkable)")
    plt.show()

# ================= GUI =================
root = tk.Tk()
root.title("Unity Map Viewer")
root.geometry("620x300")
root.resizable(False, False)

show_walkable = tk.BooleanVar(value=True)

tk.Label(root, text="UNITY MAP VIEWER", font=("Arial", 14, "bold")).pack(pady=10)

frm = tk.Frame(root)
frm.pack()

tk.Button(frm, text="Load Object JSON", width=20, command=load_object_json).grid(row=0, column=0)
tk.Button(frm, text="Load Object BIN", width=20, command=load_object_bin).grid(row=1, column=0)
tk.Button(frm, text="Load Walkable BIN", width=20, command=load_walkable_bin).grid(row=2, column=0)

lbl_json = tk.Label(frm, text="-", anchor="w", width=60)
lbl_objbin = tk.Label(frm, text="-", anchor="w", width=60)
lbl_walkbin = tk.Label(frm, text="-", anchor="w", width=60)

lbl_json.grid(row=0, column=1)
lbl_objbin.grid(row=1, column=1)
lbl_walkbin.grid(row=2, column=1)

tk.Checkbutton(
    root,
    text="Show Walkable (BLOCKED = red)",
    variable=show_walkable
).pack(pady=10)

tk.Button(
    root,
    text="VISUALIZE",
    font=("Arial", 12, "bold"),
    bg="#4CAF50",
    fg="white",
    width=25,
    command=visualize
).pack(pady=10)

root.mainloop()
