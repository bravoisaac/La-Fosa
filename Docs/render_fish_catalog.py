"""Arrange the Unity-rendered prefab previews into a labelled contact sheet."""
from pathlib import Path
import csv
from PIL import Image, ImageDraw, ImageFont

root = Path(__file__).resolve().parent.parent
rows = list(csv.DictReader((root / 'Docs/catalogo-peces.tsv').open(encoding='utf-8-sig'), delimiter='\t'))
font_dir = Path('C:/Windows/Fonts')
regular = ImageFont.truetype(str(font_dir / 'segoeui.ttf'), 17)
small = ImageFont.truetype(str(font_dir / 'segoeui.ttf'), 13)
title = ImageFont.truetype(str(font_dir / 'segoeuib.ttf'), 34)
colors = ['#8fcca5', '#68c6bd', '#78b7df', '#e1bf83', '#c5a1dc', '#8dace7']
bands = ['Intermareal', 'Costa somera', 'Pelágicos', 'Plataforma', 'Talud', 'Abisales']
width, tile_w, tile_h, margin, top = 1960, 272, 214, 24, 154
sheet = Image.new('RGB', (width, top + 7 * tile_h + 44), '#07131b')
draw = ImageDraw.Draw(sheet)
draw.text((margin, 24), 'LA FOSA  /  FAUNA DE LA COSTA CHILENA', font=title, fill='#edf6f5')
draw.text((margin, 76), '49 modelos 3D low poly · Vistas de los prefabs reales de Unity · Tamaños de vista ajustados individualmente', font=regular, fill='#abc2cc')
for i, name in enumerate(bands):
    x = margin + i * 300
    draw.ellipse((x, 116, x + 9, 125), fill=colors[i])
    draw.text((x + 18, 109), name, font=regular, fill=colors[i])
for i, row in enumerate(rows):
    x, y = margin + (i % 7) * tile_w, top + (i // 7) * tile_h
    picture = Image.open(root / 'Builds/Windows/Verification/FishModels' / (row['id'] + '.png')).convert('RGB')
    picture.thumbnail((tile_w - 12, 147), Image.Resampling.LANCZOS)
    sheet.paste(picture, (x, y))
    draw.line((x, y + 151, x + tile_w - 16, y + 151), fill=colors[int(row['franja'])], width=2)
    draw.text((x + 2, y + 157), row['nombre'], font=regular, fill='#edf6f5')
    draw.text((x + 2, y + 183), row['taxon'], font=small, fill='#92a9b6')
draw.text((margin, sheet.height - 30), 'Diseños estilizados para el prototipo. Los grupos spp. y familias se representan con un modelo ilustrativo.', font=small, fill='#92a9b6')
sheet.save(root / 'Docs/peces-catalogo.jpg', quality=94)
print(root / 'Docs/peces-catalogo.jpg')
