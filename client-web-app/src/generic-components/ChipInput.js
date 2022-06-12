import React, { useState } from 'react';
import {
    Paper,
    ListItem,
    Chip,
    TextField,
    InputAdornment,
    IconButton,
    Stack,
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import { validateEmail } from '../utils/utils';

const ChipInput = (props) => {
    const { defaultTags, onChange } = props;

    const [tags, setTags] = useState(defaultTags);
    const [input, setInput] = useState('');

    const handleDelete = (tagToDelete) => () => {
        const newTags = tags.filter((tag) => tag !== tagToDelete);
        setTags(newTags);
        onChange(newTags);
    };

    const handleAdd = (value) => {
        if (value !== '') {
            setTags([...tags, value]);
            onChange([...tags, value]);
        }
    };

    return (
        <div className='tags-input'>
            <TextField
                id='email-chip-textfield'
                margin='dense'
                type='text'
                label='Email Address'
                placeholder='Add Email Address'
                value={input}
                error={input !== '' && !validateEmail(input)}
                onChange={(e) => setInput(e.target.value)}
                onKeyUp={function (e) {
                    return (
                        e.key === 'Enter' &&
                        validateEmail(input) &&
                        handleAdd(e.target.value)
                    );
                }}
                InputProps={{
                    endAdornment: (
                        <InputAdornment position='end'>
                            <IconButton
                                id='chip-add-button'
                                aria-label='add email address'
                                onClick={() => handleAdd(input)}
                                edge='end'>
                                <AddIcon />
                            </IconButton>
                        </InputAdornment>
                    ),
                }}
            />
            <Paper
                sx={{
                    display: 'flex',
                    justifyContent: 'flex-start',
                    flexWrap: 'wrap',
                    listStyle: 'none',
                    p: 0.5,
                    m: 0,
                }}
                component='ul'>
                <Stack direction='row' spacing={1}>
                    {tags.map((email) => {
                        return (
                            <ListItem key={email}>
                                <Chip
                                    id='chip-item'
                                    label={email}
                                    onDelete={handleDelete(email)}
                                />
                            </ListItem>
                        );
                    })}
                </Stack>
            </Paper>
        </div>
    );
};

export default ChipInput;
