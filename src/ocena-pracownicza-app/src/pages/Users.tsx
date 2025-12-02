import React, { useState } from 'react';
import { 
  Box, 
  Typography, 
  Button, 
  Paper, 
  TextField, 
  InputAdornment, 
  Select, 
  MenuItem, 
  FormControl, 
  Tabs, 
  Tab,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip, 
} from '@mui/material';

import SearchIcon from '@mui/icons-material/Search';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';

const mockUsers = [
  { imie: 'Anna', nazwisko: 'Nowak', email: 'anna.nowak@firma.pl', stanowisko: 'Manager ds. Marketingu', dzial: 'Marketing', status: 'Aktywny' },
  { imie: 'Marek', nazwisko: 'Kowalczyk', email: 'marek.kowalczyk@firma.pl', stanowisko: 'Manager ds. Sprzedaży', dzial: 'Sprzedaż', status: 'Aktywny' },
  { imie: 'Tomasz', nazwisko: 'Manager', email: 'tomasz.manager@firma.pl', stanowisko: 'Kierownik IT', dzial: 'IT', status: 'Aktywny' },
];

const Users = () => {
  const [activeTab, setActiveTab] = useState(0); 
  const [department, setDepartment] = useState('Wszystkie działy');
  const [status, setStatus] = useState('Wszystkie statusy');

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setActiveTab(newValue);
  };

  return (
    <Box sx={{ flexGrow: 1 }}>
      <Box 
        sx={{ 
          display: 'flex', 
          justifyContent: 'space-between', 
          alignItems: 'center', 
          mb: 3 
        }}
      >
        <Box>
          <Typography variant="h5" component="h1" fontWeight="600">
            Zarządzanie Użytkownikami
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Przeglądaj i zarządzaj wszystkimi użytkownikami systemu
          </Typography>
        </Box>
        <Button 
          variant="contained" 
          startIcon={<AddIcon />} 
          sx={{ textTransform: 'none' }} 
        >
          Dodaj użytkownika
        </Button>
      </Box>

      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="subtitle1" fontWeight="600" sx={{ mb: 2 }}>
          Filtry
        </Typography>

        <Box sx={{ display: 'flex', gap: 2, alignItems: 'center' }}>
          <TextField
            fullWidth
            size="small"
            placeholder="Szukaj użytkownika..."
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon color="action" />
                </InputAdornment>
              ),
            }}
            sx={{ flex: 1 }}
          />
          <FormControl sx={{ minWidth: 180 }} size="small">
            <Select
              value={department}
              onChange={(e) => setDepartment(e.target.value)}
              displayEmpty
            >
              <MenuItem value="Wszystkie działy">Wszystkie działy</MenuItem>
              <MenuItem value="Marketing">Marketing</MenuItem>
              <MenuItem value="Sprzedaż">Sprzedaż</MenuItem>
            </Select>
          </FormControl>

          <FormControl sx={{ minWidth: 180 }} size="small">
            <Select
              value={status}
              onChange={(e) => setStatus(e.target.value)}
              displayEmpty
            >
              <MenuItem value="Wszystkie statusy">Wszystkie statusy</MenuItem>
              <MenuItem value="Aktywny">Aktywny</MenuItem>
              <MenuItem value="Nieaktywny">Nieaktywny</MenuItem>
            </Select>
          </FormControl>
        </Box>
      </Paper>
      <Paper sx={{ mb: 3 }}>
        <Tabs 
          value={activeTab} 
          onChange={handleTabChange} 
          indicatorColor="primary"
          textColor="primary"
          sx={{ borderBottom: 1, borderColor: 'divider' }}
        >
          <Tab 
            label="Pracownicy (4)" 
            sx={{ textTransform: 'none' }}
            disableRipple 
          />
          <Tab 
            label="Przełożeni (3)" 
            sx={{ textTransform: 'none' }}
            disableRipple 
          />
          <Tab 
            label="Administratorzy (2)" 
            sx={{ textTransform: 'none' }}
            disableRipple 
          />
        </Tabs>
      </Paper>

      <Paper sx={{ p: 3, mb: 3 }}>
        <Box sx={{ mb: 2 }}>
          <Typography variant="subtitle1" fontWeight="600">
            Lista Przełożonych
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Wszyscy menedżerowie w systemie
          </Typography>
        </Box>

        <TableContainer>
          <Table size="small">
            <TableHead>
              <TableRow>
                {['Imię', 'Nazwisko', 'E-mail', 'Stanowisko', 'Dział', 'Status', 'Akcje'].map((header) => (
                  <TableCell key={header} sx={{ color: 'text.secondary', fontWeight: '600' }}>
                    {header}
                  </TableCell>
                ))}
              </TableRow>
            </TableHead>
            <TableBody>
              {mockUsers.map((user) => (
                <TableRow 
                  key={user.email} 
                  sx={{ '&:hover': { bgcolor: 'action.hover' } }}
                >
                  <TableCell>{user.imie}</TableCell>
                  <TableCell>{user.nazwisko}</TableCell>
                  <TableCell>{user.email}</TableCell>
                  <TableCell>{user.stanowisko}</TableCell>
                  <TableCell>{user.dzial}</TableCell>
                  <TableCell>
                    <Chip
                      label={user.status}
                      size="small"
                      sx={{
                        bgcolor: '#E6F4EA',
                        color: 'success.main',
                        fontWeight: '600',
                      }}
                    />
                  </TableCell>
                  <TableCell>
                    <Box sx={{ display: 'flex', gap: 0.5 }}>
                      <EditIcon color="action" sx={{ fontSize: 18, cursor: 'pointer' }} />
                      <DeleteIcon color="error" sx={{ fontSize: 18, cursor: 'pointer' }} />
                    </Box>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>
    </Box>
  );
};

export default Users;