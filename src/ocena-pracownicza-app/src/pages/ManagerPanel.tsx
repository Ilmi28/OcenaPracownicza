import { ElementType, FC } from 'react';
import {
    Box,
    Typography,
    Button,
    Paper,
    TextField,
    InputLabel,
    Stack,
    Divider,
    Avatar
} from '@mui/material';

import { useAuth } from '../hooks/AuthProvider';

import PersonOutlineIcon from '@mui/icons-material/PersonOutline';
import MailOutlineIcon from '@mui/icons-material/MailOutline';
import WorkOutlineIcon from '@mui/icons-material/WorkOutline';

interface UserDataType {
    imie: string;
    nazwisko: string;
    email: string;
    stanowisko: string;
    dzial: string;
    wFirmieOd: string;
    ukonczoneOceny: number;
    sredniaOcena: string;
}

interface ProfileFieldProps {
    label: string;
    value: string | number;
    icon: ElementType;
}

const COLORS = {
    MAIN_BACKGROUND: '#F7F7F7',
    PAPER_BACKGROUND: '#FFFFFF',
    TEXT_PRIMARY: '#1A1A1A',
    ACCENT_BLUE: '#2D6CDF',
    TEXT_SECONDARY_GREY: '#6B7280',
    BORDER_GREY: '#D1D5DB',
};

const ProfileField: FC<ProfileFieldProps> = ({ label, value, icon: Icon }) => (
    <Box>
        <InputLabel sx={{ color: COLORS.TEXT_SECONDARY_GREY, fontSize: 12, mb: 0.5, display: 'flex', alignItems: 'center' }}>
            <Icon sx={{ fontSize: 14, mr: 0.5 }} />
            {label}
        </InputLabel>
        <TextField
            value={value}
            fullWidth
            variant="outlined"
            size="small"
            InputProps={{
                readOnly: true,
                sx: {
                    bgcolor: COLORS.PAPER_BACKGROUND,
                    borderRadius: '8px',
                    '& fieldset': { borderColor: COLORS.BORDER_GREY },
                }
            }}
            sx={{
                '& .MuiInputBase-input': { pt: '10px', pb: '10px' }
            }}
        />
    </Box>
);

const ManagerPanel: FC = () => {
    const { user, loading } = useAuth();

    if (loading) {
        return <Typography sx={{ p: 4 }}>Ładowanie danych użytkownika...</Typography>;
    }

    if (!user) {
        return <Typography sx={{ p: 4 }}>Błąd: Użytkownik nie jest zalogowany.</Typography>;
    }

    const currentData: UserDataType = {
        imie: user.name || 'Brak',
        nazwisko: user.surname || 'Brak',
        email: user.email || 'Brak',
        stanowisko: user.job || user.role || 'Pracownik',
        dzial: '-',
        wFirmieOd: '-',
        ukonczoneOceny: 0,
        sredniaOcena: '-',
    };

    return (
        <Box sx={{ p: 4, bgcolor: COLORS.MAIN_BACKGROUND, minHeight: '100%' }}>

            <Box sx={{ mb: 3 }}>
                <Typography variant="h5" component="h1" fontWeight="600" sx={{ color: COLORS.TEXT_PRIMARY }}>
                    Mój Profil
                </Typography>
                <Typography variant="body2" sx={{ color: COLORS.TEXT_SECONDARY_GREY }}>
                    Zarządzaj swoimi danymi osobowymi
                </Typography>
            </Box>

            <Paper sx={{ p: 3, mb: 3, borderRadius: '8px', bgcolor: COLORS.PAPER_BACKGROUND }}>
                <Typography variant="subtitle1" fontWeight="600" sx={{ mb: 2, color: COLORS.TEXT_PRIMARY }}>
                    Zdjęcie profilowe
                </Typography>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 3 }}>
                    <Avatar sx={{ width: 64, height: 64, bgcolor: COLORS.ACCENT_BLUE, fontSize: 24 }}>
                        {currentData.imie[0]}{currentData.nazwisko[0]}
                    </Avatar>
                    <Box>
                        <Button
                            variant="contained"
                            sx={{
                                textTransform: 'none',
                                borderRadius: '8px',
                                bgcolor: COLORS.PAPER_BACKGROUND,
                                color: COLORS.TEXT_PRIMARY,
                                border: `1px solid ${COLORS.BORDER_GREY}`,
                                boxShadow: 'none',
                                '&:hover': { bgcolor: '#f5f5f5', boxShadow: 'none' },
                                p: '6px 16px',
                                mb: 0.5
                            }}
                        >
                            Zmień zdjęcie
                        </Button>
                        <Typography variant="caption" display="block" sx={{ color: COLORS.TEXT_SECONDARY_GREY }}>
                            Zalecany format: JPG, PNG. Maksymalny rozmiar: 2MB
                        </Typography>
                    </Box>
                </Box>
            </Paper>

            <Paper sx={{ p: 3, mb: 3, borderRadius: '8px', bgcolor: COLORS.PAPER_BACKGROUND }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                    <Box>
                        <Typography variant="subtitle1" fontWeight="600" sx={{ color: COLORS.TEXT_PRIMARY }}>
                            Dane osobowe
                        </Typography>
                        <Typography variant="body2" sx={{ color: COLORS.TEXT_SECONDARY_GREY }}>
                            Podstawowe informacje o Tobie
                        </Typography>
                    </Box>
                </Box>

                <Box sx={{
                    display: 'grid',
                    gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' },
                    gap: 3
                }}>
                    <ProfileField label="Imię" value={currentData.imie} icon={PersonOutlineIcon} />
                    <ProfileField label="Nazwisko" value={currentData.nazwisko} icon={PersonOutlineIcon} />
                    <ProfileField label="E-mail" value={currentData.email} icon={MailOutlineIcon} />
                    <Box sx={{ display: { xs: 'none', sm: 'block' } }} />
                    <ProfileField label="Stanowisko" value={currentData.stanowisko} icon={WorkOutlineIcon} />
                    <ProfileField label="Dział" value={currentData.dzial} icon={WorkOutlineIcon} />
                </Box>
            </Paper>

            <Paper sx={{ p: 3, borderRadius: '8px', bgcolor: COLORS.PAPER_BACKGROUND }}>
                <Typography variant="subtitle1" fontWeight="600" sx={{ mb: 2, color: COLORS.TEXT_PRIMARY }}>
                    Statystyki
                </Typography>
                <Stack
                    direction={{ xs: 'column', sm: 'row' }}
                    divider={<Divider orientation="vertical" flexItem sx={{ borderColor: COLORS.BORDER_GREY }} />}
                    spacing={{ xs: 3, sm: 6 }}
                    justifyContent="flex-start"
                >
                    <Box sx={{ minWidth: 150 }}>
                        <Typography variant="body2" sx={{ color: COLORS.TEXT_SECONDARY_GREY, mb: 0.5 }}>
                            W firmie od
                        </Typography>
                        <Typography variant="h6" fontWeight="700" sx={{ color: COLORS.TEXT_PRIMARY }}>
                            {currentData.wFirmieOd}
                        </Typography>
                    </Box>

                    <Box sx={{ minWidth: 150 }}>
                        <Typography variant="body2" sx={{ color: COLORS.TEXT_SECONDARY_GREY, mb: 0.5 }}>
                            Ukończone oceny
                        </Typography>
                        <Typography variant="h6" fontWeight="700" sx={{ color: COLORS.TEXT_PRIMARY }}>
                            {currentData.ukonczoneOceny}
                        </Typography>
                    </Box>

                    <Box sx={{ minWidth: 150 }}>
                        <Typography variant="body2" sx={{ color: COLORS.TEXT_SECONDARY_GREY, mb: 0.5 }}>
                            Średnia ocena
                        </Typography>
                        <Typography variant="h6" fontWeight="700" sx={{ color: COLORS.TEXT_PRIMARY }}>
                            {currentData.sredniaOcena}
                        </Typography>
                    </Box>
                </Stack>
            </Paper>
        </Box>
    );
};

export default ManagerPanel;