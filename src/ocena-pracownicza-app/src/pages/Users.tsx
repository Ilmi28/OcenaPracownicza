import React, { useState, useEffect, useCallback } from 'react';
import {
    Box, Typography, Button, Paper, TextField, InputAdornment,
    Select, MenuItem, FormControl, Tabs, Tab, Table, TableBody,
    TableCell, TableContainer, TableHead, TableRow, Chip, CircularProgress, IconButton
} from '@mui/material';

import SearchIcon from '@mui/icons-material/Search';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import axiosClient from '../services/axiosClient';
import AddUserModal from '../components/AddUserModal'; // Import nowego modala

interface UserData {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    userName: string;
    position?: string;
    department?: string;
    status?: string;
}

const Users = () => {
    const [activeTab, setActiveTab] = useState(0);
    const [users, setUsers] = useState<UserData[]>([]);
    const [loading, setLoading] = useState(false);
    const [searchTerm, setSearchTerm] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false); // Stan modala

    const getEndpoint = useCallback((tabIndex: number) => {
        switch (tabIndex) {
            case 0: return '/employee';
            case 1: return '/manager';
            case 2: return '/admin';
            default: return '/employee';
        }
    }, []);

    const fetchUsers = useCallback(async () => {
        setLoading(true);
        try {
            const endpoint = getEndpoint(activeTab);
            const response = await axiosClient.get(endpoint);
            // Wyciągamy listę z pola .data (zgodnie z BaseResponse w C#)
            setUsers(response.data.data || []);
        } catch (error) {
            console.error("Błąd podczas pobierania użytkowników:", error);
            setUsers([]);
        } finally {
            setLoading(false);
        }
    }, [activeTab, getEndpoint]);

    useEffect(() => {
        fetchUsers();
    }, [fetchUsers]);

    const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
        setActiveTab(newValue);
        setSearchTerm('');
    };

    const handleDelete = async (id: string) => {
        if (window.confirm("Czy na pewno chcesz usunąć tego użytkownika?")) {
            try {
                const endpoint = getEndpoint(activeTab);
                await axiosClient.delete(`${endpoint}/${id}`);
                fetchUsers();
            } catch (error) {
                alert("Wystąpił błąd podczas usuwania użytkownika.");
            }
        }
    };

    const filteredUsers = users.filter(user =>
        `${user.firstName} ${user.lastName} ${user.email}`.toLowerCase().includes(searchTerm.toLowerCase())
    );

    return (
        <Box sx={{ flexGrow: 1 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
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
                    sx={{ textTransform: 'none', borderRadius: '8px' }}
                    onClick={() => setIsModalOpen(true)} // Otwieranie modala
                >
                    Dodaj użytkownika
                </Button>
            </Box>

            <Paper sx={{ p: 2, mb: 3, borderRadius: '12px' }} elevation={0} variant="outlined">
                <Typography variant="subtitle2" fontWeight="600" sx={{ mb: 1.5 }}>Filtry</Typography>
                <Box sx={{ display: 'flex', gap: 2 }}>
                    <TextField
                        fullWidth
                        size="small"
                        placeholder="Szukaj użytkownika..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        InputProps={{
                            startAdornment: (
                                <InputAdornment position="start">
                                    <SearchIcon color="action" fontSize="small" />
                                </InputAdornment>
                            ),
                        }}
                    />
                    <FormControl sx={{ minWidth: 150 }} size="small">
                        <Select displayEmpty value="all">
                            <MenuItem value="all">Wszystkie działy</MenuItem>
                            <MenuItem value="it">IT</MenuItem>
                            <MenuItem value="marketing">Marketing</MenuItem>
                        </Select>
                    </FormControl>
                    <FormControl sx={{ minWidth: 150 }} size="small">
                        <Select displayEmpty value="all">
                            <MenuItem value="all">Wszystkie statusy</MenuItem>
                            <MenuItem value="active">Aktywny</MenuItem>
                            <MenuItem value="inactive">Nieaktywny</MenuItem>
                        </Select>
                    </FormControl>
                </Box>
            </Paper>

            <Paper sx={{ mb: 0, borderRadius: '12px 12px 0 0' }} elevation={0} variant="outlined">
                <Tabs
                    value={activeTab}
                    onChange={handleTabChange}
                    indicatorColor="primary"
                    textColor="primary"
                    sx={{ px: 2 }}
                >
                    <Tab label="Pracownicy" sx={{ textTransform: 'none', fontWeight: '600' }} />
                    <Tab label="Przełożeni" sx={{ textTransform: 'none', fontWeight: '600' }} />
                    <Tab label="Administratorzy" sx={{ textTransform: 'none', fontWeight: '600' }} />
                </Tabs>
            </Paper>

            <TableContainer component={Paper} elevation={0} variant="outlined" sx={{ borderRadius: '0 0 12px 12px', borderTop: 0 }}>
                {loading ? (
                    <Box sx={{ display: 'flex', justifyContent: 'center', p: 5 }}><CircularProgress size={24} /></Box>
                ) : (
                    <Table size="small">
                        <TableHead sx={{ bgcolor: '#F9FAFB' }}>
                            <TableRow>
                                <TableCell sx={{ fontWeight: '600', color: 'text.secondary' }}>Imię</TableCell>
                                <TableCell sx={{ fontWeight: '600', color: 'text.secondary' }}>Nazwisko</TableCell>
                                <TableCell sx={{ fontWeight: '600', color: 'text.secondary' }}>E-mail</TableCell>
                                <TableCell sx={{ fontWeight: '600', color: 'text.secondary' }}>Stanowisko</TableCell>
                                <TableCell sx={{ fontWeight: '600', color: 'text.secondary' }}>Status</TableCell>
                                <TableCell align="right" sx={{ fontWeight: '600', color: 'text.secondary' }}>Akcje</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {filteredUsers.length > 0 ? filteredUsers.map((user: any) => (
                                <TableRow key={user.id} hover>
                                    <TableCell>{user.firstName}</TableCell>
                                    <TableCell>{user.lastName}</TableCell>
                                    <TableCell>{user.email || 'brak danych'}</TableCell>
                                    <TableCell>
                                        {user.position || (activeTab === 1 ? 'Menedżer' : 'Administrator')}
                                    </TableCell>
                                    <TableCell>
                                        <Chip
                                            label="Aktywny"
                                            size="small"
                                            sx={{ bgcolor: '#E6F4EA', color: 'success.main', fontWeight: '600' }}
                                        />
                                    </TableCell>
                                    <TableCell align="right">
                                        <IconButton size="small"><EditIcon fontSize="small" color="action" /></IconButton>
                                        <IconButton size="small" onClick={() => handleDelete(user.id)}><DeleteIcon fontSize="small" color="error" /></IconButton>
                                    </TableCell>
                                </TableRow>
                            )) : (
                                <TableRow><TableCell colSpan={6} align="center" sx={{ py: 3 }}>Brak danych do wyświetlenia</TableCell></TableRow>
                            )}
                        </TableBody>
                    </Table>
                )}
            </TableContainer>

            {/* Komponent Modala */}
            <AddUserModal
                open={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                activeTab={activeTab}
                onSuccess={fetchUsers}
            />
        </Box>
    );
};

export default Users;