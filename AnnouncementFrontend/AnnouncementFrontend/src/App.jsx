import React, { useEffect, useState } from "react";
import {
  Container,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,  
  TextField,
  DialogActions,
  IconButton
} from "@mui/material";
import { Edit, Delete, Add } from "@mui/icons-material";
import axios from "axios";
import Header from "./Header";


const API_URL = 'https://localhost:7141/Announcement';

const App = () => {
  const [announcements, setAnnouncements] = useState([]);
  const [selected, setSelected] = useState(null);
  const [openDialog, setOpenDialog] = useState(false);
  const [formData, setFormData] = useState({ title: "", description: "" });

  const fetchAnnouncements = async () => {
    const response = await axios.get(API_URL);
    setAnnouncements(response.data);
  };

  useEffect(() => {
    fetchAnnouncements();
  }, []);

  const handleSelect = async (id) => {
    const response = await axios.get(`${API_URL}/details/${id}`);
    setSelected(response.data);
    setFormData({ title: response.data.title, description: response.data.description });
    setOpenDialog(true);
  };

  const handleDelete = async (id) => {
    await axios.delete(`${API_URL}/${id}`);
    setOpenDialog(false);
    fetchAnnouncements();
  };

  const handleAdd = () => {
    setSelected(null);
    setFormData({ title: "", description: "" });
    setOpenDialog(true);
  };

  const handleSave = async () => {
    try {
      if (selected?.isEditing) {
        await axios.put(`${API_URL}/${selected.id}`, {
          id: selected.id,
          title: formData.title,
          description: formData.description
        });
      } else {
        await axios.post(API_URL, formData);
      }
      setOpenDialog(false);
      setSelected(null);
      fetchAnnouncements();
    } catch (error) {
      console.error("Error saving announcement:", error);
    }
  };

  const handleEdit = () => {
    setFormData({ 
      title: selected.title, 
      description: selected.description 
    });
    setSelected({ ...selected, isEditing: true });
  };

  const formatDate = (utcDate, includeTime = false) => {
    const localDate = new Date(utcDate);
    
    const options = {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      ...(includeTime && {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        hour12: false,
      })
    };

    return new Intl.DateTimeFormat('uk-UA', options).format(localDate);
  };

  return (
    <>
      <Header/>
      <Container maxWidth="lg" sx={{ mt: 4 }}>
        <Button 
          variant="contained" 
          startIcon={<Add />} 
          onClick={handleAdd} 
          sx={{ 
            mb: 4,
            backgroundColor: '#f79a24',
            '&:hover': {
              backgroundColor: '#e08910'
            }
          }}
        >
          Додати оголошення
        </Button>

        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Оголошення</TableCell>
              <TableCell>Дата додавання</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {announcements.map((a) => (
              <TableRow 
                key={a.id} 
                onClick={() => handleSelect(a.id)}
                sx={{ cursor: 'pointer', '&:hover': { backgroundColor: '#f5f5f5' } }}
              >
                <TableCell>{a.title}</TableCell>
                <TableCell>{formatDate(a.dateAdded)}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>

        <Dialog 
          open={openDialog} 
          onClose={() => setOpenDialog(false)}
          maxWidth="md"
          fullWidth
        >
          <DialogTitle>
            {selected ? (selected.isEditing ? "Редагувати оголошення" : "Подробиці оголошення") : "Додати оголошення"}
          </DialogTitle>
          <DialogContent>
            {selected ? (
              selected.isEditing ? (
                <>
                  <TextField
                    margin="dense"
                    label="Заголовок"
                    fullWidth
                    value={formData.title}
                    onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                  />
                  <TextField
                    margin="dense"
                    label="Опис"
                    fullWidth
                    multiline
                    minRows={3}
                    value={formData.description}
                    onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                  />
                </>
              ) : (
                <div>
                  <Typography variant="h6" gutterBottom>{selected.title}</Typography>
                  <Typography variant="body1" paragraph>{selected.description}</Typography>
                  <Typography variant="subtitle2" color="textSecondary">
                    Дата додавання: {formatDate(selected.dateAdded, true)}
                  </Typography>
                  
                  <div style={{ marginTop: '1rem' }}>
                    <IconButton onClick={handleEdit}><Edit /></IconButton>
                    <IconButton onClick={() => handleDelete(selected.id)}><Delete /></IconButton>
                  </div>

                  {selected.similarAnnouncements?.length > 0 && (
                    <div style={{ marginTop: '2rem', borderTop: '1px solid #e0e0e0', paddingTop: '1rem' }}>
                      <Typography variant="h6" sx={{ mb: 2, color: '#666' }}>
                        Схожі оголошення:
                      </Typography>
                      <div style={{ display: 'grid', gap: '1rem' }}>
                        {selected.similarAnnouncements.map((s) => (
                          <div 
                            key={s.id} 
                            style={{ 
                              padding: '1rem',
                              backgroundColor: '#f8f9fa',
                              borderRadius: '8px',
                              cursor: 'pointer'
                            }}
                            onClick={() => handleSelect(s.id)}
                          >
                            <Typography variant="subtitle1" sx={{ fontWeight: 'bold' }}>
                              {s.title}
                            </Typography>
                            <Typography variant="body2" color="text.secondary">
                              Додано: {formatDate(s.dateAdded)}
                            </Typography>
                          </div>
                        ))}
                      </div>
                    </div>
                  )}
                </div>
              )
            ) : (
              <>
                <TextField
                  margin="dense"
                  label="Заголовок"
                  fullWidth
                  value={formData.title}
                  onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                />
                <TextField
                  margin="dense"
                  label="Опис"
                  fullWidth
                  multiline
                  minRows={3}
                  value={formData.description}
                  onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                />
              </>
            )}
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setOpenDialog(false)}>Закрити</Button>
            {(!selected || selected.isEditing) && 
              <Button onClick={handleSave} variant="contained">Зберегти</Button>
            }
          </DialogActions>
        </Dialog>
      </Container>
    </>
  );
};

export default App;