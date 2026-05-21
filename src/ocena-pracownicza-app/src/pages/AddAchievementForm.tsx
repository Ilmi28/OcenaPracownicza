import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import axiosClient from "../services/axiosClient";

export type AchievementModel = {
    name: string;
    description: string;
    date: string;
    employeeId: string;
    category: number;
    evaluationPeriodId: string;
    finalScore: string;
    achievementsSummary: string;
    isDraft: boolean;
};

interface CurrentUser {
    id: string;
    firstName: string;
    lastName: string;
}

interface ApiResponse<T> {
    success: boolean;
    message: string;
    data: T;
}

interface EvaluationPeriodBasicInfo {
    id: string;
    name: string;
}

type ApiError = {
    response?: {
        data?: {
            message?: string;
        };
    };
    message?: string;
};

type Props = {
    initialEmployeeId?: string;
    onSuccess?: () => void;
};

const KATEGORIE = [
    { id: 1, nazwa: "Sukces projektowy" },
    { id: 2, nazwa: "Rozwój techniczny" },
    { id: 3, nazwa: "Innowacja" },
    { id: 4, nazwa: "Mentoring" },
    { id: 5, nazwa: "Inne" },
];

const formatToLocal = (iso: string) => iso.slice(0, 16);
const getCategoryName = (categoryId: number) =>
    KATEGORIE.find((category) => category.id === categoryId)?.nazwa ?? "Inne";
const getErrorMessage = (error: unknown, fallback: string) => {
    const apiError = error as ApiError;
    return apiError.response?.data?.message || apiError.message || fallback;
};

const AddAchievementForm: React.FC<Props> = ({
    initialEmployeeId = "",
    onSuccess,
}) => {
    const navigate = useNavigate();
    const [currentUser, setCurrentUser] = useState<CurrentUser | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [loadingUser, setLoadingUser] = useState(true);
    const [detectedPeriod, setDetectedPeriod] = useState<string | null>("Pobieranie...");
    const [isConfirmationOpen, setIsConfirmationOpen] = useState(false);
    const [isCompletenessConfirmed, setIsCompletenessConfirmed] = useState(false);

    const [message, setMessage] = useState<{
        type: "success" | "error";
        text: string;
    } | null>(null);

    const [form, setForm] = useState<AchievementModel>({
        name: "",
        description: "",
        date: new Date().toISOString(),
        employeeId: initialEmployeeId,
        category: 1,
        evaluationPeriodId: "",
        finalScore: "",
        achievementsSummary: "",
        isDraft: false,
    });

    useEffect(() => {
        const fetchCurrentUserInfo = async () => {
            try {
                const resp = await axiosClient.get<ApiResponse<CurrentUser>>("/employee/me");
                const userData = resp.data.data;
                
                setCurrentUser(userData);

                setForm((prev) => ({
                    ...prev,
                    employeeId: userData.id,
                }));
            } catch (error) {
                console.error("Błąd pobierania profilu użytkownika:", error);
                setMessage({ 
                    type: "error", 
                    text: "Nie udało się pobrać danych Twojego profilu." 
                });
            } finally {
                setLoadingUser(false);
            }
        };
        fetchCurrentUserInfo();
    }, []);


useEffect(() => {
    const fetchPeriod = async () => {
        try {
            const resp = await axiosClient.get<EvaluationPeriodBasicInfo>(
                `/evaluation-periods/by-date?date=${form.date}`
            );
            
            const { id, name } = resp.data;

            setDetectedPeriod(name); 
            
            setForm(prev => ({
                ...prev,
                evaluationPeriodId: id             
            }));
        } catch {
            setDetectedPeriod(null);
            setForm(prev => ({ ...prev, evaluationPeriodId: "" }));
        }
    };
    
    if (form.date) {
        fetchPeriod();
    }
}, [form.date]);

    const handleChange = (
        e: React.ChangeEvent<
            HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
        >,
    ) => {
        const { name, value } = e.target;
        setForm((prev) => ({
            ...prev,
            [name]:
                name === "category"
                    ? Number(value)
                    : name === "date"
                      ? new Date(value).toISOString()
                      : value,
        }));
    };

    const isFormComplete =
        Boolean(form.name.trim()) &&
        Boolean(form.description.trim()) &&
        Boolean(form.date) &&
        Boolean(form.employeeId) &&
        Boolean(form.evaluationPeriodId) &&
        Boolean(form.finalScore.trim()) &&
        Boolean(form.achievementsSummary.trim()) &&
        Boolean(detectedPeriod);

    const submitForm = async (isDraft: boolean) => {
        if (!detectedPeriod) {
            setMessage({ type: "error", text: "Nie można zapisać: Wybrana data nie pasuje do żadnego okresu ocen." });
            return;
        }

        setIsSubmitting(true);
        setMessage(null);
        
        const payload = { ...form, isDraft };

        try {
            await axiosClient.post("/achievement", payload);
            if (onSuccess) onSuccess();
            navigate("/achievements");
        } catch (error) {
            const errorMsg = getErrorMessage(
                error,
                "Nie udało się zapisać osiągnięcia.",
            );
            setMessage({ type: "error", text: errorMsg });
            setIsSubmitting(false);
        }
    };

    const handleDraftSubmit = () => submitForm(true);
    const handleFinalSubmit = () => {
        if (!isFormComplete) {
            setMessage({
                type: "error",
                text: "Uzupełnij wszystkie wymagane pola przed wysłaniem zgłoszenia do przełożonego.",
            });
            return;
        }

        setIsCompletenessConfirmed(false);
        setIsConfirmationOpen(true);
        setMessage(null);
    };

    const handleConfirmFinalSubmit = () => {
        submitForm(false);
        setIsConfirmationOpen(false);
    };

    return (
        <form className="achievement-form">
            <div className="form-header">
                <button
                    type="button"
                    onClick={() => navigate(-1)}
                    className="back-btn"
                >
                    ← Wróć
                </button>
                <h2>Dodaj Osiągnięcie</h2>
            </div>

            {message && (
                <div className={`alert ${message.type}`}>{message.text}</div>
            )}

            <div className="form-group">
                {loadingUser ? (
                    <div className="loading-placeholder">Pobieranie danych profilu...</div>
                ) : (
                    <div className="logged-user-info">
                        <span className="user-name">
                            {currentUser ? `${currentUser.firstName} ${currentUser.lastName}` : "Błąd! Zaloguj się ponownie!"}
                        </span>
                        <input type="hidden" name="employeeId" value={form.employeeId} />
                    </div>
                )}
            </div>

            {                     }
            <div className="form-group">
                <label htmlFor="name">Nazwa osiągnięcia</label>
                <input
                    id="name"
                    name="name"
                    type="text"
                    value={form.name}
                    onChange={handleChange}
                    required
                />
            </div>

            <div className="form-group">
                <label htmlFor="description">Opis</label>
                <textarea
                    id="description"
                    name="description"
                    value={form.description}
                    onChange={handleChange}
                    required
                    rows={4}
                />
            </div>

<div className="form-group">
    <label htmlFor="date">Data i godzina</label>
    <input
        id="date"
        name="date"
        type="datetime-local"
        value={formatToLocal(form.date)}
        onChange={handleChange}
        required
    />
    
    {            }
    <div style={{ 
        marginTop: '8px', 
        fontSize: '0.9rem', 
        padding: '10px', 
        borderRadius: '6px',
        backgroundColor: detectedPeriod ? '#f0fff4' : '#fff5f5',
        color: detectedPeriod ? '#276749' : '#c53030',
        border: `1px solid ${detectedPeriod ? '#c6f6d5' : '#feb2b2'}`,
        fontWeight: '600'
    }}>
        {detectedPeriod === "Pobieranie..." ? (
            <span>🔍 Sprawdzanie okresu...</span>
        ) : detectedPeriod ? (
            <span>📅 Przypisany okres: {detectedPeriod}</span>
        ) : (
            <span>⚠️ Nieznany okres - zmień datę, aby móc zapisać!</span>
        )}
    </div>
</div>

            <div className="form-group">
                <label htmlFor="category">Kategoria</label>
                <select
                    id="category"
                    name="category"
                    value={form.category}
                    onChange={handleChange}
                >
                    {KATEGORIE.map((kat) => (
                        <option key={kat.id} value={kat.id}>
                            {kat.nazwa}
                        </option>
                    ))}
                </select>
            </div>

            <div className="form-group">
                <label htmlFor="finalScore">Wynik końcowy</label>
                <input
                    id="finalScore"
                    name="finalScore"
                    type="text"
                    value={form.finalScore}
                    onChange={handleChange}
                    required
                />
            </div>

            <div className="form-group">
                <label htmlFor="achievementsSummary">Podsumowanie osiągnięć</label>
                <textarea
                    id="achievementsSummary"
                    name="achievementsSummary"
                    value={form.achievementsSummary}
                    onChange={handleChange}
                    required
                    rows={3}
                />
            </div>
            <div className="button-group" style={{ display: 'flex', gap: '1rem', marginTop: '1rem' }}>
                <button
                    type="button"
                    className="submit-btn draft-btn"
                    onClick={handleDraftSubmit}
                    disabled={isSubmitting || loadingUser || !detectedPeriod}
                    style={{ background: '#718096' }} // Szary kolor wyróżniający szkic
                >
                    {isSubmitting ? "Zapisywanie..." : "Zapisz jako wersję roboczą"}
                </button>

                <button
                    type="button"
                    className="submit-btn"
                    onClick={handleFinalSubmit}
                    disabled={isSubmitting || loadingUser || !detectedPeriod}
                >
                    {isSubmitting ? "Zapisywanie..." : "Prześlij do oceny"}
                </button>
            </div>

            {isConfirmationOpen && (
                <div className="confirmation-overlay" role="dialog" aria-modal="true" aria-labelledby="submission-confirmation-title">
                    <div className="confirmation-card">
                        <h3 id="submission-confirmation-title">Potwierdź kompletność zgłoszenia</h3>
                        <p className="confirmation-copy">
                            Zweryfikuj dane przed wysłaniem do przełożonego.
                        </p>

                        <dl className="confirmation-summary">
                            <div>
                                <dt>Nazwa</dt>
                                <dd>{form.name}</dd>
                            </div>
                            <div>
                                <dt>Kategoria</dt>
                                <dd>{getCategoryName(form.category)}</dd>
                            </div>
                            <div>
                                <dt>Data</dt>
                                <dd>{new Date(form.date).toLocaleString("pl-PL")}</dd>
                            </div>
                            <div>
                                <dt>Okres</dt>
                                <dd>{detectedPeriod}</dd>
                            </div>
                            <div>
                                <dt>Wynik końcowy</dt>
                                <dd>{form.finalScore}</dd>
                            </div>
                            <div>
                                <dt>Opis</dt>
                                <dd>{form.description}</dd>
                            </div>
                            <div>
                                <dt>Podsumowanie</dt>
                                <dd>{form.achievementsSummary}</dd>
                            </div>
                        </dl>

                        <label className="confirmation-checkbox">
                            <input
                                type="checkbox"
                                checked={isCompletenessConfirmed}
                                onChange={(event) =>
                                    setIsCompletenessConfirmed(event.target.checked)
                                }
                            />
                            <span>
                                Potwierdzam, że zgłoszenie jest kompletne i gotowe do wysłania do przełożonego.
                            </span>
                        </label>

                        <div className="confirmation-actions">
                            <button
                                type="button"
                                className="secondary-btn"
                                onClick={() => setIsConfirmationOpen(false)}
                                disabled={isSubmitting}
                            >
                                Wróć do edycji
                            </button>
                            <button
                                type="button"
                                className="submit-btn"
                                onClick={handleConfirmFinalSubmit}
                                disabled={!isCompletenessConfirmed || isSubmitting}
                            >
                                {isSubmitting ? "Zapisywanie..." : "Potwierdź i wyślij"}
                            </button>
                        </div>
                    </div>
                </div>
            )}

            <style>{`
                /* Style pozostają takie same jak w Twoim poprzednim fragmencie */
                .achievement-form {
                    max-width: 550px;
                    margin: 2rem auto;
                    display: grid;
                    gap: 1.25rem;
                    padding: 2.5rem;
                    background: #ffffff;
                    border-radius: 12px;
                    box-shadow: 0 10px 25px rgba(0,0,0,0.05);
                    font-family: 'Inter', sans-serif;
                }
                .form-header { display: flex; align-items: center; gap: 1rem; margin-bottom: 0.5rem; }
                .back-btn { background: none; border: none; color: #4e73df; cursor: pointer; font-weight: 600; }
                .form-group { display: flex; flex-direction: column; gap: 0.4rem; }
                label { font-size: 0.85rem; font-weight: 700; color: #4e5d78; }
                input, textarea, select { 
                    padding: 12px; border: 1px solid #dce1e8; border-radius: 8px; 
                    background-color: #f9fbff; color: #2e3b4e; font-size: 0.95rem;
                }
                .logged-user-info {
                    padding: 12px;
                    background-color: #edf2f7;
                    border: 1px solid #e2e8f0;
                    border-radius: 8px;
                }
                .user-name { font-weight: 600; color: #2d3748; }
                .submit-btn { 
                    margin-top: 1rem; padding: 14px; cursor: pointer; background: #4e73df; 
                    color: white; border: none; border-radius: 8px; font-weight: 700; font-size: 1rem;
                }
                .secondary-btn {
                    margin-top: 1rem;
                    padding: 14px;
                    cursor: pointer;
                    background: #edf2f7;
                    color: #2d3748;
                    border: 1px solid #dce1e8;
                    border-radius: 8px;
                    font-weight: 700;
                    font-size: 1rem;
                }
                .submit-btn:disabled,
                .secondary-btn:disabled {
                    cursor: not-allowed;
                    opacity: 0.6;
                }
                .alert.error { background: #fed7d7; color: #822727; padding: 10px; border-radius: 5px; }
                .confirmation-overlay {
                    position: fixed;
                    inset: 0;
                    background: rgba(15, 23, 42, 0.45);
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    padding: 1.5rem;
                }
                .confirmation-card {
                    width: min(100%, 640px);
                    background: #ffffff;
                    border-radius: 16px;
                    padding: 1.5rem;
                    box-shadow: 0 24px 48px rgba(15, 23, 42, 0.2);
                    display: grid;
                    gap: 1rem;
                }
                .confirmation-copy {
                    margin: 0;
                    color: #4a5568;
                }
                .confirmation-summary {
                    margin: 0;
                    display: grid;
                    gap: 0.75rem;
                }
                .confirmation-summary div {
                    padding: 0.75rem;
                    border-radius: 10px;
                    background: #f8fafc;
                }
                .confirmation-summary dt {
                    font-size: 0.8rem;
                    font-weight: 700;
                    color: #4a5568;
                    margin-bottom: 0.25rem;
                }
                .confirmation-summary dd {
                    margin: 0;
                    color: #1f2937;
                    white-space: pre-wrap;
                    word-break: break-word;
                }
                .confirmation-checkbox {
                    display: flex;
                    align-items: flex-start;
                    gap: 0.75rem;
                    color: #2d3748;
                    font-weight: 500;
                }
                .confirmation-checkbox input {
                    margin-top: 0.2rem;
                }
                .confirmation-actions {
                    display: flex;
                    justify-content: flex-end;
                    gap: 0.75rem;
                }
            `}</style>
        </form>
    );
};

export default AddAchievementForm;
