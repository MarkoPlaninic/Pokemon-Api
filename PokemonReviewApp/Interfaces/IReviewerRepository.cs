﻿using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces;

public interface IReviewerRepository
{
    ICollection<Reviewer> GetReviewers();
    Reviewer GetReviewer(int revieverId);
    ICollection<Review> GetReviewsByReviewer(int reviewerId);
    bool ReviewerExists(int id);
    bool CreateReviewer(Reviewer reviewer);
    bool Save();
}