import { Box, Typography, Paper, Avatar, Button, TextField, Grid, Chip } from '@mui/material';

const AdminProfile = () => {
    const adminData = {
        firstName: "Adam",
        lastName: "Administratorski",
        email: "admin@firma.pl",
        role: "Administrator"
    };

    return (
        <Box>
            <Typography variant="h5" fontWeight="600">Mój Profil</Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>Zarządzaj swoimi danymi osobowymi</Typography>

            <Paper sx={{ p: 3, mb: 3 }}>
                <Typography variant="subtitle1" fontWeight="600" mb={2}>Zdjęcie profilowe</Typography>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 3 }}>
                    <Avatar sx={{ width: 80, height: 80, bgcolor: 'primary.main', fontSize: '2rem' }}>AA</Avatar>
                    <Box>
                        <Chip label={adminData.role} size="small" color="primary" variant="outlined" sx={{ mb: 1 }} />
                        <br />
                        <Button variant="outlined" size="small">Zmień zdjęcie</Button>
                    </Box>
                </Box>
            </Paper>

            <Paper sx={{ p: 3 }}>
                <Typography variant="subtitle1" fontWeight="600" mb={2}>Dane osobowe</Typography>
                {/* Usunięcie 'item' i użycie 'size' lub 'xs' bezpośrednio na Gridzie */}
                <Grid container spacing={3}>
                    <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField fullWidth label="Imię" value={adminData.firstName} disabled size="small" />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField fullWidth label="Nazwisko" value={adminData.lastName} disabled size="small" />
                    </Grid>
                    <Grid size={{ xs: 12 }}>
                        <TextField fullWidth label="E-mail" value={adminData.email} disabled size="small" />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField fullWidth label="Stanowisko" value="Administrator" disabled size="small" />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField fullWidth label="Dział" value="IT" disabled size="small" />
                    </Grid>
                </Grid>
            </Paper>
        </Box>
    );
};

export default AdminProfile;